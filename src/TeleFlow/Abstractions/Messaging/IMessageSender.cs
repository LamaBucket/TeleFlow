using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Messaging;

public interface IMessageSender
{
    Task<Message> SendMessage(OutgoingMessage message);
}