using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Transport.Messaging;

public interface IMessageSender
{
    Task<Message> SendMessage(OutgoingMessage message);
}