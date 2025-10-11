using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services;

public class FromUpdateChatIdProvider : IChatIdProvider
{
    private readonly long _chatId;

    public long GetChatId()
    {
        return _chatId;
    }

    public FromUpdateChatIdProvider(Update update)
    {
        _chatId = update.GetChatId();
    }
}
