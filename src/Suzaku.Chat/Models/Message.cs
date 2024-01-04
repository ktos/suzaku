using System.Text.Json.Serialization;

namespace Suzaku.Chat.Models
{
    [JsonDerivedType(typeof(Message), "message")]
    [JsonDerivedType(typeof(Busy), "busy")]
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
