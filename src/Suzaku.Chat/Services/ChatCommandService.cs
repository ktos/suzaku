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
            if (message == "/new" || message == "/busy" || message.StartsWith("/rename"))
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
            else if (command == "/busy")
            {
                return new Busy { Sender = "User", Timestamp = DateTime.UtcNow };
            }
            else if (command.StartsWith("/rename "))
            {
                string newName = command.Substring("/rename ".Length);
                _chatHistory.CurrentChannel.DisplayName = newName;
                _chatHistory.UpdatedByCommand();
            }

            return null;
        }
    }
}
