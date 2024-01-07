using Suzaku.Bot.Services;

namespace ExampleBot.Services;

internal class BasicPromptProvider : IPromptProvider
{
    public string SystemPrompt { get; } = "You are a helpful assistant.";
}
