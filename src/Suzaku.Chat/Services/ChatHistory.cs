using Microsoft.Extensions.Options;
using Suzaku.Chat.Models;
using Suzaku.Shared;
using System.Text.Json;

namespace Suzaku.Chat.Services
{
	/// <summary>
	/// The main system tracking the history of the all conversations in all channels
	/// </summary>
	public class ChatHistory
	{
		private const string HISTORY_FILE_PATH = "history.json";

		private List<ChatChannel> _channels;
		private readonly ChannelConfiguration _configuration;
		private ChatChannel _currentChannel;

		/// <summary>
		/// The channel which is currently active in the chat window
		/// </summary>
		public ChatChannel CurrentChannel
		{
			get { return _currentChannel; }
			private set { _currentChannel = value; ChannelHistoryUpdated?.Invoke(); }
		}

		/// <summary>
		/// The default channel, default group chat, without a internal name, published to a suzaku/chat topic
		/// </summary>
		public ChatChannel DefaultChannel => _channels.First(x => x.Name is null);

		/// <summary>
		/// Fired when the channel history has been updated (by the new message usually)
		/// </summary>
		public event Func<Task>? ChannelHistoryUpdated;

		/// <summary>
		/// Fired when the channel list has been modified, by the channel rename command
		/// </summary>
		public event Func<Task>? ChannelRenamed;

		public ChatHistory(IOptions<ChannelConfiguration> configuration)
		{
			_channels = [];
			_configuration = configuration.Value;

			if (File.Exists(HISTORY_FILE_PATH))
			{
				_channels = JsonSerializer.Deserialize<List<ChatChannel>>(File.ReadAllText(HISTORY_FILE_PATH)) ?? [];
			}

			// if the history was empty, create at least a default channel
			if (_channels is [])
				_channels.Add(new ChatChannel { Name = null, DisplayName = configuration.Value.DefaultChannelName, CurrentConversationId = Guid.NewGuid(), History = new List<Element>() });

			_currentChannel = _channels.First();
		}

		/// <summary>
		/// Sets the default group chat as the current channel
		/// </summary>
		public void SetDefaultChannelAsCurrent()
		{
			CurrentChannel = _channels.First(x => x.Name is null);
		}

		/// <summary>
		/// Sets the channel with a given internal name as a current channel
		/// </summary>
		/// <param name="name"></param>
		public void SetChannelAsCurrent(string? name)
		{
			CurrentChannel = FindByName(name);
		}

		private ChatChannel CreateNewChannelByName(string name)
		{
			var tryAlways = _configuration.AlwaysDisplayed.FirstOrDefault(x => x.Name == name);

			ChatChannel created;

			if (tryAlways != null)
			{
				created = new ChatChannel { Name = tryAlways.Name, DisplayName = tryAlways.DisplayName, CurrentConversationId = Guid.NewGuid(), History = new List<Element>() };
			}
			else
			{
				created = new ChatChannel { Name = name.ToNormalizedChannelName(), DisplayName = name, CurrentConversationId = Guid.NewGuid(), History = new List<Element>() };
			}

			_channels.Add(created);
			return created;
		}

		private ChatChannel FindByName(string? channelName)
		{
			// channel name is null for a default channel (group chat with all bots)
			if (channelName == null)
			{
				return _channels.First(x => x.Name is null);
			}

			var found = _channels.FirstOrDefault(x => x.Name == channelName);
			return found ?? CreateNewChannelByName(channelName);
		}

		/// <summary>
		/// Adds a new element to a history of a given channel name
		/// </summary>
		/// <param name="element"></param>
		/// <param name="channelName"></param>
		public void AddElement(Element element, string? channelName)
		{
			var c = FindByName(channelName);
			c.History.Add(element);
			ChannelHistoryUpdated?.Invoke();
			SaveHistory();
		}

		/// <summary>
		/// <para>Adds a new message to a history of a given channel name</para>
		/// <para>Additionally, it updates the current conversation id if needed</para>
		/// <para>If the last message was a list of canned responses, and the message is from the user, they are being marked as interacted with</para>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="channelName"></param>
		public void AddMessage(Message message, string? channelName)
		{
			var c = FindByName(channelName);

			// if a new conversation id, start a new conversation
			if (message.ConversationId is not null && message.ConversationId != c.CurrentConversationId)
			{
				c.CurrentConversationId = message.ConversationId.Value;
				var marker = new NewConversationMarker
				{
					Id = Guid.NewGuid(),
					Timestamp = DateTime.UtcNow
				};

				AddElement(marker, channelName);
			}

			// if there is a message from user and there are waiting non-interacted canned responses
			// mark them as interacted already
			if (message.Sender == "User")
			{
				c.History.Where(x => x is CannedResponses can && !can.IsInteracted).Select(x => x as CannedResponses).ToList().ForEach(x => x!.IsInteracted = true);
			}

			AddElement(message, channelName);
		}

		/// <summary>
		/// Returns the current channels history
		/// </summary>
		/// <returns></returns>
		public List<Element> GetCurrentChannelElements()
		{
			return _currentChannel.History.ToList();
		}

		/// <summary>
		/// Returns all of the recorded channels apart from the default one
		/// </summary>
		/// <returns></returns>
		public List<ChatChannel> GetChannels()
		{
			return _channels.Where(x => x.Name is not null).ToList();
		}

		/// <summary>
		/// Adds a "busy" message to the given channel
		/// </summary>
		/// <param name="chat"></param>
		/// <param name="channelName"></param>
		public void AddBusyMessage(Busy chat, string? channelName)
		{
			var c = FindByName(channelName);

			var last = c.History.Where(x => x is Busy busy && busy.Sender == chat.Sender).FirstOrDefault();
			if (last == null)
			{
				c.History.Add(chat);
			}

			ChannelHistoryUpdated?.Invoke();
			SaveHistory();
		}

		/// <summary>
		/// Removes a "busy" message from a given channel for a given sender
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="channelName"></param>
		public void RemoveBusyMessagesForSender(string sender, string? channelName)
		{
			var c = FindByName(channelName);

			var last = c.History.Where(x => x is Busy busy && busy.Sender == sender).FirstOrDefault();
			if (last != null)
			{
				c.History.Remove(last);
			}

			ChannelHistoryUpdated?.Invoke();
			SaveHistory();
		}

		/// <summary>
		/// Should be run when command updated the channel history or name, fires events
		/// </summary>
		public void UpdatedByCommand()
		{
			ChannelHistoryUpdated?.Invoke();
			ChannelRenamed?.Invoke();
			SaveHistory();
		}

		private void SaveHistory()
		{
			File.WriteAllText(HISTORY_FILE_PATH, JsonSerializer.Serialize(_channels));
		}

		/// <summary>
		/// Introduces a new conversation id for a given channel
		/// </summary>
		/// <param name="channelName"></param>
		/// <param name="guid"></param>
		public void NewConversationForChannel(string? channelName, Guid? guid = null)
		{
			var ch = FindByName(channelName);
			ch.CurrentConversationId = guid ?? Guid.NewGuid();

			var marker = new NewConversationMarker
			{
				Id = Guid.NewGuid(),
				Timestamp = DateTime.UtcNow
			};

			AddElement(marker, channelName);
		}
	}
}
