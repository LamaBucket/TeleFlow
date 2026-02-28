using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Transport.Messaging;

public static class MessageSendServiceExtensions
{
    public static Task<Message> SendMessage(this IMessageSendService messageSender, string message)
    {
        var msg = InlineMarkupMessage.CreateTextMessage(message);

        return messageSender.SendMessage(msg);
    }
}