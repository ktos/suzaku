namespace Suzaku.Chat.Models
{
    public class ChatChannel
    {
        public string? Name { get; set; }

        public required string DisplayName { get; set; }

        public Guid CurrentConversationId { get; set; }

        public IList<Element> History { get; set; } = null!;
    }
}
