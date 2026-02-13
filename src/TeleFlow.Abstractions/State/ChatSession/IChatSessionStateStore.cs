namespace TeleFlow.Abstractions.State.ChatSession;

public interface IChatSessionStore
{
    Task<ChatSession?> GetAsync(long chatId);

    Task SetAsync(long chatId, ChatSession session, TimeSpan? ttl = null);

    Task RemoveAsync(long chatId);
}