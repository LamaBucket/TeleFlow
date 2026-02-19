using TeleFlow.Abstractions.State.Chat;

namespace TeleFlow.Extensions.DI.Implementations.State.Chat;

public class InMemoryChatSessionStore : IChatSessionStore
{
    private readonly Dictionary<long, ChatSession> _sessions = [];

    public async Task<ChatSession?> GetAsync(long chatId)
    {
        return _sessions.GetValueOrDefault(chatId);
    }

    public async Task RemoveAsync(long chatId)
    {
        _sessions.Remove(chatId);
    }

    public async Task SetAsync(long chatId, ChatSession session, TimeSpan? ttl = null)
    {
        if(!_sessions.TryAdd(chatId, session))
            _sessions[chatId] = session;
    }
}