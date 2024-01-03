using System.Text.Json.Serialization;

namespace Suzaku.Chat.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid? Response { get; set; }

        public string? Content { get; set; }

        public DateTime? Timestamp { get; set; }

        public User Sender { get; set; }

        public bool BusyMarker { get; set; } = false;
    }

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

    public enum User
    {
        Human,
        Suza,
        Saga
    }

    public static class Formatter
    {
        public static string ToTopic(User recipient)
        {
            switch (recipient)
            {
                case User.Saga:
                case User.Suza:
                    return "suzaku/chat";

                default:
                    throw new ArgumentException("You cannot send message to this recipient");
            }
        }

        public static User FromTopic(string topic)
        {
            switch (topic)
            {
                case "suzaku/saga/chat_response":
                case "suzaku/saga/chat_system":
                    return User.Saga;

                case "suzaku/suza/chat_response":
                case "suzaku/suza/chat_system":
                    return User.Suza;

                default:
                    throw new ArgumentException("Message coming from unknown topic");
            }
        }
    }
}
