using Suzaku.Chat.Models;

namespace Suzaku.Chat.Services
{
    public class ChatCommandService
    {
        public Guid ConversationId { get; private set; }

        public ChatCommandService()
        {
            ConversationId = Guid.NewGuid();
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
                ConversationId = Guid.NewGuid();
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
