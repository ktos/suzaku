﻿using System.Text.Json.Serialization;

namespace Suzaku.Chat.Models
{
    [JsonDerivedType(typeof(Message), "message")]
    [JsonDerivedType(typeof(Busy), "busy")]
    [JsonDerivedType(typeof(Attachment), "attachment")]
    [JsonDerivedType(typeof(NewConversationMarker), "new-conv")]
    public abstract class Element
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ChatName { get; set; }
    }

    public class NewConversationMarker : Element;

    public class Message : Element
    {
        public Guid? ConversationId { get; set; }

        public string? Content { get; set; }

        public required string Sender { get; set; }
    }

    public class Busy : Message;

    public class Attachment : Message;
}
