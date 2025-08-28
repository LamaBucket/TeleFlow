using LisBot.Common.Telegram.Services;
using Telegram.Bot;

namespace demo.Services;

public class ReplyMarkupManagerFactory : IReplyMarkupManagerFactory
{
    private readonly ITelegramBotClient _botClient;

    public IReplyMarkupManager CreateReplyMarkupManager(long chatId)
    {
        return new ReplyMarkupManager(chatId, _botClient);
    }

    public ReplyMarkupManagerFactory(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
}