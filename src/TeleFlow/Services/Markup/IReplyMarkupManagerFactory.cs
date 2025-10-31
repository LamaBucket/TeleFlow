namespace TeleFlow.Services.Markup;

public interface IReplyMarkupManagerFactory
{
    IReplyMarkupManager CreateReplyMarkupManager(long chatId);
}