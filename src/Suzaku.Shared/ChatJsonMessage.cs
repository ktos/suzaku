using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Suzaku.Shared
{
    public class ChatJsonMessage
    {
        [JsonPropertyName("conversation_id")]
        public required Guid ConversationId { get; set; }

        [JsonPropertyName("sender")]
        public required string Sender { get; set; }

        [JsonPropertyName("content")]
        public required string Content { get; set; }

        public const string ATTACHMENT = "file:";
        public const string CANNED = "canned:";
    }

    public class SystemJsonMessage
    {
        [JsonPropertyName("sender")]
        public required string Sender { get; set; }

        [JsonPropertyName("content")]
        public required string Content { get; set; }

        public const string BUSY = "BUSY";
        public const string NOT_BUSY = "NOT_BUSY";
        public const string NEW_CONVERSATION = "NEWCONV";
    }
}
