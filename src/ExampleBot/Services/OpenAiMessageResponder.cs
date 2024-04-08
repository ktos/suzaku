using OpenAiNg;
using OpenAiNg.Chat;
using Suzaku.Bot.Services;

namespace Suza.Services
{
    internal class OpenAiMessageResponder : IMessageResponder
    {
        private readonly OpenAiApi _api;
        private readonly string _systemPrompt;

        public OpenAiMessageResponder(OpenAiApi api, IPromptProvider promptProvider)
        {
            _api = api;
            _systemPrompt = promptProvider.SystemPrompt;
        }

        public async Task<string?> HandleFileUploadedAsync(
            string sender,
            string fileName,
            Guid conversationId,
            bool isPrivate
        )
        {
            return null;
        }

        public async Task<string?> RespondAsync(
            string sender,
            string message,
            Guid conversationId,
            bool isPrivate
        )
        {
            if (sender != "User")
                return null;

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, _systemPrompt),
                new ChatMessage(ChatMessageRole.User, message),
            };

            var result = await _api.Chat.CreateChatCompletionAsync(messages, temperature: 0.9);

            if (result.Choices != null && result.Choices.FirstOrDefault() != null)
            {
                return result.Choices.FirstOrDefault().Message?.Content;
            }

            return null;
        }
    }
}
