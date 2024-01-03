using Suzaku.Chat.Models;
using System;
using System.Collections.ObjectModel;

namespace Suzaku.Chat.Services
{
	public class ChatHistory
	{
		private ObservableCollection<Element> _chatHistory;
		public event Func<Task>? Notify;

		public ChatHistory()
		{
			_chatHistory = [];
		}

		public void AddMessage(Message message)
		{
			_chatHistory.Add(message);
			Notify?.Invoke();
		}

		public ObservableCollection<Element> GetAllElements()
		{
			return _chatHistory;
		}

		public void AddBusyMessage(Busy chat)
		{
			var last = _chatHistory.Where(x => x is Busy busy && busy.Sender == chat.Sender).FirstOrDefault();
			if (last == null)
			{
				_chatHistory.Add(chat);
			}

			Notify?.Invoke();
		}

		internal void RemoveBusyMessagesForSender(string sender)
		{
			var last = _chatHistory.Where(x => x is Busy busy && busy.Sender == sender).FirstOrDefault();
			if (last != null)
			{
				_chatHistory.Remove(last);
			}

			Notify?.Invoke();
		}
	}
}
