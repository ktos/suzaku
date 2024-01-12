namespace Suzaku.Chat.Models
{
    /// <summary>
    /// Configuration for the human user of the system
    /// </summary>
    public class UserConfiguration
    {
        /// <summary>
        /// Displayed name of the user
        /// </summary>
        public string Name { get; set; } = "User";

        /// <summary>
        /// URL of the user's avatar
        /// </summary>
        public string Avatar { get; set; } = "/avatars/user.png";
    }

    /// <summary>
    /// Configuration of the channel displayed in the chat window
    /// </summary>
    public class ChannelConfiguration
    {
        /// <summary>
        /// Should channels from the history be displayed, or only the static list
        /// </summary>
        public bool DisplayFromHistory { get; set; } = true;

        /// <summary>
        /// Displayed name of the default chat
        /// </summary>
        public string DefaultChannelName { get; set; } = "Default";

        /// <summary>
        /// Static list of the channels to be displayed in the beginning of the list, before the history
        /// </summary>
        public IEnumerable<ChatChannelConfiguration> AlwaysDisplayed { get; set; } =
            new List<ChatChannelConfiguration>();
    }

    /// <summary>
    /// Configuration of the displayed channel for the channel list
    /// </summary>
    public class ChatChannelConfiguration
    {
        /// <summary>
        /// Internal name of the channel, included in topic
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Displayed name of the channel
        /// </summary>
        public string DisplayName { get; set; } = null!;
    }
}
