namespace LisBot.Common.Telegram.Services;

public interface IMessageServiceFactory<TMessage>
{
    IMessageService<TMessage> CreateMessageService(long chatId);
}