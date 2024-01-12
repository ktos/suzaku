
namespace Suzaku.Chat.Services
{
	public interface ICommunicationService
	{
		Task InitializeAsync();
		Task PublishUserAttachmentAsync(string fileName, Guid conversationId, string? chatName);
		Task PublishUserMessage(string content, Guid conversationId, string? chatName);
	}
}