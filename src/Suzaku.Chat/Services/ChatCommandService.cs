using Suzaku.Chat.Models;

namespace Suzaku.Chat.Services
{
    /// <summary>
    /// A service handling commands (messages starting with /) send by the user
    /// </summary>
    public class ChatCommandService
    {
        private readonly ChatHistory _chatHistory;
        private readonly ICommunicationService _mqttService;
        private readonly FileHandler _fileHandler;

        public ChatCommandService(
            ChatHistory chatHistory,
            ICommunicationService communicationService,
            FileHandler fileHandler
        )
        {
            _chatHistory = chatHistory;
            _mqttService = communicationService;
            _fileHandler = fileHandler;
        }

        /// <summary>
        /// Checks if a given message text is a command
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool IsCommand(string message)
        {
            if (
                message == "/new"
                || message == "/busy"
                || message.StartsWith("/rename")
                || message.StartsWith("/attach")
            )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes command and optionally returns element to be included in a chat window
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<Element?> ExecuteCommandAsync(string command)
        {
            if (command == "/new")
            {
                _chatHistory.CurrentChannel.CurrentConversationId = Guid.NewGuid();
                return new NewConversationMarker { Timestamp = DateTime.UtcNow };
            }
            else if (command == "/busy")
            {
                return new Busy { Sender = "User", Timestamp = DateTime.UtcNow };
            }
            else if (command.StartsWith("/rename "))
            {
                _chatHistory.CurrentChannel.DisplayName = command.Substring("/rename ".Length);
                _chatHistory.UpdatedByCommand();
            }
            else if (command.StartsWith("/attach "))
            {
                var param = command.Substring("/attach ".Length).Trim();
                var result = await _fileHandler.HandleAttachmentFromUriAsync(param);

                if (result != null)
                {
                    await _mqttService.PublishUserAttachmentAsync(
                        result,
                        _chatHistory.CurrentChannel.CurrentConversationId,
                        _chatHistory.CurrentChannel.Name
                    );
                }
                else
                {
                    return new Error { Content = $"Attaching failed (param: {param})" };
                }
            }
            return null;
        }
    }
}
