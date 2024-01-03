using OpenAiNg;
using OpenAiNg.Chat;
using Suzaku.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suza.Services
{
    internal class BasicMessageResponder : IMessageResponder
    {
        private readonly OpenAiApi _api;
        private readonly string _systemPrompt;
        private readonly string _botInstruction;

        public BasicMessageResponder(OpenAiApi api, PromptProvider promptProvider)
        {
            _api = api;
            _systemPrompt = promptProvider.SystemPrompt;
            _botInstruction = promptProvider.PassToTheHumanInstructionPrompt;
        }

        public async Task<string?> RespondAsync(string? message)
        {
            if (message == null)
            {
                return null;
            }

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, _systemPrompt),
                new ChatMessage(ChatMessageRole.User, message),
            };

            var result = await _api.Chat.CreateChatCompletionAsync(messages, temperature: 1.1);

            if (result.Choices != null && result.Choices.FirstOrDefault() != null)
            {
                return result.Choices.FirstOrDefault().Message?.Content;
            }

            return null;
        }

        public async Task<string?> SummarizeBotMessageAsync(string sender, string? message)
        {
            if (message == null)
            {
                return null;
            }

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, _systemPrompt),
                new ChatMessage(
                    ChatMessageRole.User,
                    string.Format(_botInstruction, sender) + "\n\n" + message
                ),
            };

            var result = await _api.Chat.CreateChatCompletionAsync(messages, temperature: 1.1);

            if (result.Choices != null && result.Choices.FirstOrDefault() != null)
            {
                return result.Choices.FirstOrDefault().Message?.Content;
            }

            return null;
        }
    }
}
