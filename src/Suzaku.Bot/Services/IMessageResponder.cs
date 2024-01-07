namespace Suzaku.Bot.Services
{
    public interface IMessageResponder
    {
        Task<string?> RespondAsync(string sender, string message, Guid conversationId);
    }
}
