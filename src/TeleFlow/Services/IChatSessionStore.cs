using TeleFlow.Models;

namespace TeleFlow.Services;

public interface IChatSessionStore
{
    Task<ChatSession?> GetAsync(long chatId);

    Task SetAsync(ChatSession session, TimeSpan? ttl = null);

    Task RemoveAsync(long chatId);
}