using Suzaku.Chat.Models;
using System.Text.Json;

namespace Suzaku.Chat.Services
{
    public class ChatHistory
    {
        private const string HISTORY_FILE_PATH = "history.json";

        private List<ChatChannel> _channels;

        private ChatChannel _currentChannel;

        public ChatChannel CurrentChannel
        {
            get { return _currentChannel; }
            private set { _currentChannel = value; Notify?.Invoke(); }
        }


        public event Func<Task>? Notify;

        public ChatHistory()
        {
            _channels = [];

            if (File.Exists(HISTORY_FILE_PATH))
            {
                _channels = JsonSerializer.Deserialize<List<ChatChannel>>(File.ReadAllText(HISTORY_FILE_PATH)) ?? [];
            }

            if (_channels is [])
                _channels.Add(new ChatChannel { Name = null, DisplayName = "Default", CurrentConversationId = Guid.NewGuid(), History = new List<Element>() });

            _currentChannel = _channels.First();
        }

        public void SetDefaultChannelAsCurrent()
        {
            CurrentChannel = _channels.First(x => x.Name is null);
        }

        public void SetChannelAsCurrent(string? name)
        {
            CurrentChannel = FindByName(name);
        }

        private ChatChannel CreateNewChannel(string name)
        {
            var newch = new ChatChannel { Name = name.ToLower(), DisplayName = name, CurrentConversationId = Guid.NewGuid(), History = new List<Element>() };
            _channels.Add(newch);
            return newch;
        }

        private ChatChannel FindByName(string? channelName)
        {
            // channel name is null for a default channel (group chat with all bots)
            if (channelName == null)
            {
                return _channels.First(x => x.Name is null);
            }

            var found = _channels.FirstOrDefault(x => x.Name == channelName);
            return found ?? CreateNewChannel(channelName);
        }

        public void AddElement(Element element, string? channelName)
        {
            var c = FindByName(channelName);
            c.History.Add(element);
            Notify?.Invoke();
            SaveHistory();
        }

        public void AddMessage(Message message, string? channelName)
        {
            var c = FindByName(channelName);
            c.History.Add(message);
            Notify?.Invoke();
            SaveHistory();
        }

        public List<Element> GetCurrentChannelElements()
        {
            return _currentChannel.History.ToList();
        }

        public List<ChatChannel> GetChannels()
        {
            return _channels.Where(x => x.Name is not null).ToList();
        }

        public void AddBusyMessage(Busy chat, string? channelName)
        {
            var c = FindByName(channelName);

            var last = c.History.Where(x => x is Busy busy && busy.Sender == chat.Sender).FirstOrDefault();
            if (last == null)
            {
                c.History.Add(chat);
            }

            Notify?.Invoke();
            SaveHistory();
        }

        public void RemoveBusyMessagesForSender(string sender, string? channelName)
        {
            var c = FindByName(channelName);

            var last = c.History.Where(x => x is Busy busy && busy.Sender == sender).FirstOrDefault();
            if (last != null)
            {
                c.History.Remove(last);
            }

            Notify?.Invoke();
            SaveHistory();
        }

        private void SaveHistory()
        {
            File.WriteAllText(HISTORY_FILE_PATH, JsonSerializer.Serialize(_channels));
        }
    }
}
