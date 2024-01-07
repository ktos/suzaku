using Suzaku.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuzaBot.Services
{
    internal class FakeMessageResponder : HistoryAwareMessageResponder
    {
        public override async Task<string?> RespondAsync(
            string sender,
            string message,
            Guid conversationId
        )
        {
            if (sender != "User")
            {
                return null;
            }

            UpdateHistory(sender, message, conversationId);

            await Task.Delay(1000);
            return $"Hey {sender}, conversation id is {conversationId}!\n\nPrevious messages: "
                + GetHistoryAsString(conversationId);
        }
    }
}
