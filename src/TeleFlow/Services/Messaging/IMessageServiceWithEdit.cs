using Telegram.Bot.Types;

namespace TeleFlow.Services.Messaging;

public interface IMessageServiceWithEdit<TMessage> : IMessageService<TMessage>
{
    Task<Message> EditMessage(int messageId, TMessage message);
}