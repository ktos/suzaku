using Microsoft.Extensions.Options;
using Suzaku.Bot.Models;
using Suzaku.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleBot.Services
{
    internal class TestMessageResponder : HistoryAwareMessageResponder
    {
        private readonly string _name;

        public TestMessageResponder(IOptions<BotConfiguration> options)
        {
            _name = options.Value.Name;
        }

        public override async Task<string?> RespondAsync(
            string sender,
            string message,
            Guid conversationId
        )
        {
            if (sender != "User" || IsMessageToMe(message))
            {
                return null;
            }

            UpdateHistory(sender, message, conversationId);

            await Task.Delay(1000);
            return $"Hey {sender}, conversation id is {conversationId}!\n\nPrevious messages: "
                + GetHistoryAsString(conversationId);
        }

        private bool IsMessageToMe(string message)
        {
            return message.Contains($"@{_name}") || message.StartsWith(@"{_name}");
        }
    }
}
