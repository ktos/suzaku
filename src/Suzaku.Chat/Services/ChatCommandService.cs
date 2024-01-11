using Suzaku.Chat.Models;

namespace Suzaku.Chat.Services
{
    public class ChatCommandService
    {
        private readonly ChatHistory _chatHistory;

        public ChatCommandService(ChatHistory chatHistory)
        {
            _chatHistory = chatHistory;
        }

        public bool IsCommand(string message)
        {
            if (new string[] { "/new", "/busy" }.Contains(message))
            {
                return true;
            }

            return false;
        }

        public Element? ParseCommand(string command)
        {
            if (command == "/new")
            {
                _chatHistory.CurrentChannel.CurrentConversationId = Guid.NewGuid();
                return new NewConversationMarker { Timestamp = DateTime.UtcNow };
            }

            if (command == "/busy")
            {
                return new Busy { Sender = "User", Timestamp = DateTime.UtcNow };
            }

            return null;
        }
    }
}
