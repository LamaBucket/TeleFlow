namespace TeleFlow.Abstractions.State.Chat;

public interface IChatSessionStateStore
{
    Task<ChatSession?> GetAsync(long chatId);

    Task SetAsync(long chatId, ChatSession session, TimeSpan? ttl = null);

    Task RemoveAsync(long chatId);
}