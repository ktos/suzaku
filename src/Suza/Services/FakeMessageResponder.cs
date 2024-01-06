using Suzaku.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuzaBot.Services
{
    internal class FakeMessageResponder : IMessageResponder
    {
        private readonly ConversationHistory _history;

        public FakeMessageResponder()
        {
            _history = new ConversationHistory();
        }

        public async Task<string?> RespondAsync(string sender, string message, Guid conversationId)
        {
            if (sender != "User")
            {
                return null;
            }

            var conversationHistory = _history.GetHistory(conversationId);
            string history = string.Empty;
            if (conversationHistory != null)
            {
                foreach (var item in conversationHistory)
                {
                    history += $"{item.Item1}:{item.Item2}\n";
                }
            }
            _history.AddToHistory(sender, message, conversationId);

            await Task.Delay(1000);
            return $"Hey {sender}, conversation is {conversationId}!\n\nPrevious messages: "
                + history;
        }
    }
}
