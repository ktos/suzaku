using Suzaku.Chat.Models;
using System.Collections.ObjectModel;

namespace Suzaku.Chat.Services
{
	public class ChatHistory
	{
		private ObservableCollection<ChatMessage> _chatMessages;
		public event Func<Task>? Notify;

		public ChatHistory()
		{
			_chatMessages = [];
		}

		public void AddMessage(ChatMessage message)
		{
			var last = _chatMessages.OrderByDescending(x => x.Timestamp).FirstOrDefault();
			if (last != null && last.BusyMarker && message.Sender == last.Sender)
			{
				_chatMessages.Remove(last);
			}

			_chatMessages.Add(message);
			Notify?.Invoke();
		}

		public ObservableCollection<ChatMessage> GetAllMessages()
		{
			return _chatMessages;
		}
	}
}
