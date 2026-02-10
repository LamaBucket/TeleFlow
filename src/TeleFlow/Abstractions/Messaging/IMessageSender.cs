using TeleFlow.Models.Messaging;
using Telegram.Bot.Types;

namespace TeleFlow.Services.Messaging;

public interface IMessageSender
{
    Task<Message> SendMessage(OutgoingMessage message);
}