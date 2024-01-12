namespace Suzaku.Chat.Services
{
    /// <summary>
    /// A communication services used to communicate between the user and the bots
    /// </summary>
    public interface ICommunicationService
    {
        /// <summary>
        /// Initializes a communication, connects to the server etc.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        /// Publishes the file onto the given chat channel
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="conversationId"></param>
        /// <param name="channel">null is the default chat</param>
        /// <returns></returns>
        Task PublishUserAttachmentAsync(string fileName, Guid conversationId, string? channel);

        /// <summary>
        /// Publishes the message as the human user to the chat, onto a given channel
        /// </summary>
        /// <param name="content"></param>
        /// <param name="conversationId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task PublishUserMessageAsync(string content, Guid conversationId, string? channel);

        /// <summary>
        /// Publishes the message as the human user to a currently active conversation on a currently active channel
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task PublishUserMessageOnCurrentChannelAndConversationAsync(string content);
    }
}
