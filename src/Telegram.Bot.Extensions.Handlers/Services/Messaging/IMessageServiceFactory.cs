namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;


public interface IMessageServiceFactory<TMessageService, TMessage> where TMessageService : IMessageService<TMessage>
{
    TMessageService CreateMessageService(long chatId);
}

public interface IMessageServiceFactory<TMessage> : IMessageServiceFactory<IMessageService<TMessage>, TMessage>
{
}