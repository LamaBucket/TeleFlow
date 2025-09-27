using Telegram.Bot.Extensions.Handlers.Services.Messaging;

namespace LisBot.Common.Telegram.Commands;

public class SendTextCommand : SendMessageCommand<string>
{
    public SendTextCommand(IMessageService<string> messageService, string message) : base(messageService, message)
    {
    }
}
