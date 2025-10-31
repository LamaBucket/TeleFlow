using Telegram.Bot.Types;

namespace TeleFlow.Services.Messaging;

public interface IMessageService<TMessage>
{
    Task<Message> SendMessage(TMessage message);
}