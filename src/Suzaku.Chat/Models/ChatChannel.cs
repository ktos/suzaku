namespace Suzaku.Chat.Models
{
    /// <summary>
    /// A channel on which there is a conversation between the User and other systems
    /// </summary>
    public class ChatChannel
    {
        /// <summary>
        /// <para>Name of the channel</para>
        /// <para>
        /// null is the default group chat, published on suzaku/chat topic, in other cases
        /// messages are being published to suzaku/{name}/chat topic
        /// </para>
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Displayed name of the channel
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// Current conversation id, used to track context
        /// </summary>
        public Guid CurrentConversationId { get; set; }

        /// <summary>
        /// List of all elements (messages, attachments, and so on) that were on the current channel
        /// </summary>
        public IList<Element> History { get; set; } = null!;
    }
}
