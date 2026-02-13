using TeleFlow.Abstractions.Engine.ChatIdentity;

namespace TeleFlow.Extensions.DependencyInjection.Implementations.Engine;

public class InMemoryChatIdManager : IChatIdSetter, IChatIdProvider
{
    private long? _chatId;

    public long GetChatId()
    {
        return _chatId ?? throw new InvalidOperationException("Chat ID has not been set.");
    }

    public void SetChatId(long chatId)
    {
        _chatId = chatId;
    }
}