using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Models;

public interface IArgsWithMessageService
{
    IMessageServiceWithEdit<Message> MessageService { get; }
}