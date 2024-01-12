namespace Suzaku.Chat.Services
{
    public interface ICommunicationService
    {
        Task InitializeAsync();
        Task PublishUserAttachmentAsync(string fileName, Guid conversationId, string? chatName);
        Task PublishUserMessageAsync(string content, Guid conversationId, string? chatName);
        Task PublishUserMessageAsync(string content);
    }
}
