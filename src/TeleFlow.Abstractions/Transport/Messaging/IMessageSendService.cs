using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Transport.Messaging;

public interface IMessageSendService
{
    Task<Message> SendMessage(InlineMarkupMessage message);

    Task<Message> SendMessage(ReplyMarkupMessage message);
}