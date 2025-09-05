using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public interface IMessageServiceWithEdit<TMessage> : IMessageService<TMessage>
{
    Task<Message> EditMessage(int messageId, TMessage message);
}