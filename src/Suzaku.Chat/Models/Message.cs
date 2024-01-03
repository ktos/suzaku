using System.Text.Json.Serialization;

namespace Suzaku.Chat.Models
{
    public abstract class Element
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Message : Element
    {
        public Guid? ConversationId { get; set; }

        public string? Content { get; set; }

        public required string Sender { get; set; }
    }

    public class Busy : Message { }
}
