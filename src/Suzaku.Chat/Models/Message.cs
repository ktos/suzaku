using System.Text.Json.Serialization;

namespace Suzaku.Chat.Models
{
    [JsonDerivedType(typeof(Message), "message")]
    [JsonDerivedType(typeof(Busy), "busy")]
    [JsonDerivedType(typeof(Attachment), "attachment")]
    [JsonDerivedType(typeof(NewConversationMarker), "new-conv")]
    [JsonDerivedType(typeof(Error), "error")]
    public abstract class Element
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class NewConversationMarker : Element;

    public class Error : Element
    {
        public string? Content { get; set; }
    }

    public class Message : Element
    {
        public Guid? ConversationId { get; set; }

        public string? Content { get; set; }

        public required string Sender { get; set; }
    }

    public class Busy : Message;

    public class Attachment : Message;
}
