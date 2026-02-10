namespace TeleFlow.Abstractions.Sessions;

public interface IChatSessionStore
{
    Task<ChatSession?> GetAsync(long chatId);

    Task SetAsync(long chatId, ChatSession session, TimeSpan? ttl = null);

    Task RemoveAsync(long chatId);
}