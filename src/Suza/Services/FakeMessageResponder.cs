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
        public async Task<string?> RespondAsync(string sender, string message)
        {
            if (sender != "User")
            {
                return null;
            }

            await Task.Delay(1000);
            return $"Hey {sender}!";
        }
    }
}
