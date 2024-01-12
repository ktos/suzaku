using System.Text.Json.Serialization;

namespace Suzaku.Chat.Models
{
    /// <summary>
    /// The base class for the type of the element displayed in the chat window
    /// </summary>
    [JsonDerivedType(typeof(Message), "message")]
    [JsonDerivedType(typeof(Busy), "busy")]
    [JsonDerivedType(typeof(Attachment), "attachment")]
    [JsonDerivedType(typeof(NewConversationMarker), "new-conv")]
    [JsonDerivedType(typeof(Error), "error")]
    [JsonDerivedType(typeof(Info), "info")]
    [JsonDerivedType(typeof(CannedResponses), "canned")]
    public abstract class Element
    {
        /// <summary>
        /// Element ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Element timestamp, as UTC time
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Class marking there was a message meaning the new conversation happened
    /// and conversation id was changed
    /// </summary>
    public class NewConversationMarker : Element;

    /// <summary>
    /// Class marking there was some kind of a system error during the communication
    /// </summary>
    public class Error : Element
    {
        /// <summary>
        /// Error message to be shown to the user
        /// </summary>
        public string? Content { get; set; }
    }

    /// <summary>
    /// Class marking the generic information from the system to the user
    /// </summary>
    public class Info : Element
    {
        /// <summary>
        /// Message to be shown to the user
        /// </summary>
        public string? Content { get; set; }
    }

    /// <summary>
    /// A piece of conversation
    /// </summary>
    public class Message : Element
    {
        /// <summary>
        /// Conversation id, used for tracking context between messages
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Content of the message
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Text name of the message's sender
        /// </summary>
        public required string Sender { get; set; }
    }

    /// <summary>
    /// A marker showing "busy" information from the given sender
    /// </summary>
    public class Busy : Message;

    /// <summary>
    /// A message with a link to the file
    /// </summary>
    public class Attachment : Message;

    /// <summary>
    /// A message with a list of options to use
    /// </summary>
    public class CannedResponses : Message
    {
        public List<string> Responses { get; set; } = [];
        public bool IsInteracted { get; set; }
    }
}
