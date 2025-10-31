using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;

namespace TeleFlow.Models;

public interface IArgsWithMessageService
{
    IMessageServiceWithEdit<Message> MessageService { get; }
}