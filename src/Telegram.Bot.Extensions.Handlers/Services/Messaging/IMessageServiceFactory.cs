namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public interface IMessageServiceFactory<TMessage>
{
    IMessageService<TMessage> CreateMessageService(long chatId);
}