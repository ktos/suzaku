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
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("response_id")]
        public Guid? Response { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("busy")]
        public bool BusyMarker { get; set; } = false;
    }

    public class SystemJsonMessage
    {
        [JsonPropertyName("sender")]
        public string? Sender { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("busy")]
        public bool BusyMarker { get; set; } = false;
    }
}
