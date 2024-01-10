using Suzaku.Chat.Models;
using System.Text.Json;

namespace Suzaku.Chat.Services
{
	public class ChatHistory
	{
		private const string HISTORY_FILE_PATH = "history.json";
		private List<Element> _chatHistory;
		public event Func<Task>? Notify;

		public ChatHistory()
		{
			_chatHistory = [];

			if (File.Exists(HISTORY_FILE_PATH))
			{
				var history = JsonSerializer.Deserialize<IEnumerable<Element>>(File.ReadAllText(HISTORY_FILE_PATH));
				if (history != null)
				{
					foreach (var item in history)
					{
						_chatHistory.Add(item);
					}
					Notify?.Invoke();
				}
			}
		}

		public void AddElement(Element element)
		{
			_chatHistory.Add(element);
			Notify?.Invoke();
			SaveHistory();
		}

		public void AddMessage(Message message)
		{
			_chatHistory.Add(message);
			Notify?.Invoke();
			SaveHistory();
		}

		public List<Element> GetAllElements(string? chatName = null)
		{
			if (chatName == null)
			{
				return _chatHistory.Where(x => x.ChatName is null).ToList();
			}
			else
			{
				return _chatHistory.Where(x => x.ChatName == chatName).ToList();
			}
		}

		public void AddBusyMessage(Busy chat)
		{
			var last = _chatHistory.Where(x => x is Busy busy && busy.Sender == chat.Sender).FirstOrDefault();
			if (last == null)
			{
				_chatHistory.Add(chat);
			}

			Notify?.Invoke();
			SaveHistory();
		}

		public void RemoveBusyMessagesForSender(string sender)
		{
			var last = _chatHistory.Where(x => x is Busy busy && busy.Sender == sender).FirstOrDefault();
			if (last != null)
			{
				_chatHistory.Remove(last);
			}

			Notify?.Invoke();
			SaveHistory();
		}

		private void SaveHistory()
		{
			File.WriteAllText(HISTORY_FILE_PATH, JsonSerializer.Serialize(_chatHistory));
		}
	}
}
