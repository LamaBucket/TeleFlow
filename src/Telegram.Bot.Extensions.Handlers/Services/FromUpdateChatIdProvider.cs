using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Services;

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
