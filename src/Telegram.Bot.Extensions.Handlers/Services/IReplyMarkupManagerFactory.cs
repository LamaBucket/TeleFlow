namespace LisBot.Common.Telegram.Services;

public interface IReplyMarkupManagerFactory
{
    IReplyMarkupManager CreateReplyMarkupManager(long chatId);
}