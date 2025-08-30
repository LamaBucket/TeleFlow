namespace Telegram.Bot.Extensions.Handlers.Services.Markup;

public interface IReplyMarkupManagerFactory
{
    IReplyMarkupManager CreateReplyMarkupManager(long chatId);
}