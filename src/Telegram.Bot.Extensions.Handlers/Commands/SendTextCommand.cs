using Telegram.Bot.Extensions.Handlers.Services.Messaging;

namespace Telegram.Bot.Extensions.Handlers.Commands;

public class SendTextCommand : SendMessageCommand<string>
{
    public SendTextCommand(IMessageService<string> messageService, string message) : base(messageService, message)
    {
    }
}
