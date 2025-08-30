using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public interface IMessageService<TMessage>
{
    Task<Message> SendMessage(TMessage message);
}