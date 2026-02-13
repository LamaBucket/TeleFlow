using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Transport.Messaging;

public static class MessageSenderExtensions
{
    public static Task<Message> SendMessage(this IMessageSender messageSender, string message)
    {
        var msg = OutgoingMessage.CreateTextMessage(message);

        return messageSender.SendMessage(msg);
    }
}