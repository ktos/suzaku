using Suzaku.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzaku.Bot.Services
{
    public abstract class HistoryAwareMessageResponder : IMessageResponder
    {
        protected readonly ConversationHistory _history;

        public HistoryAwareMessageResponder()
        {
            _history = new ConversationHistory();
        }

        protected void UpdateHistory(string sender, string message, Guid conversationId)
        {
            _history.AddToHistory(sender, message, conversationId);
        }

        protected string GetHistoryAsString(Guid conversationId)
        {
            var conversationHistory = _history.GetHistory(conversationId);

            var sb = new StringBuilder();
            if (conversationHistory != null)
            {
                foreach (var item in conversationHistory)
                {
                    sb.AppendLine($"{item.Item1}:{item.Item2}");
                }
            }

            return sb.ToString();
        }

        public abstract Task<string?> RespondAsync(
            string sender,
            string message,
            Guid conversationId
        );
    }
}
