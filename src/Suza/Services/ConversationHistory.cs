namespace SuzaBot.Services
{
    class ConversationHistory
    {
        private readonly Dictionary<Guid, Queue<(string, string)>> _history = new();

        public void AddToHistory(string sender, string message, Guid conversationId)
        {
            if (!_history.ContainsKey(conversationId))
            {
                _history.Add(conversationId, new Queue<(string, string)>());
            }

            _history[conversationId].Enqueue((sender, message));
        }

        public IEnumerable<(string, string)>? GetHistory(Guid conversationId)
        {
            if (_history.ContainsKey(conversationId))
            {
                return _history[conversationId];
            }

            return null;
        }
    }
}
