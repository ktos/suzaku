namespace Suzaku.Bot.Services
{
    public interface IMessageResponder
    {
        Task<string?> RespondAsync(
            string sender,
            string message,
            Guid conversationId,
            bool isPrivate
        );

        Task<string?> HandleFileUploadedAsync(
            string sender,
            string fileName,
            Guid conversationId,
            bool isPrivate
        );
    }
}
