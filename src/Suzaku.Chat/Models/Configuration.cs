namespace Suzaku.Chat.Models
{
    public class UserConfiguration
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
    }

    public class ChannelConfiguration
    {
        public bool DisplayFromHistory { get; set; } = true;

        public string DefaultChannelName { get; set; } = "Default";

        public IEnumerable<ChatChannelConfiguration> AlwaysDisplayed { get; set; } =
            new List<ChatChannelConfiguration>();
    }

    public class ChatChannelConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
