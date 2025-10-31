using TeleFlow.Services.Messaging;

namespace TeleFlow.Commands;

public class SendTextCommand : SendMessageCommand<string>
{
    public SendTextCommand(IMessageService<string> messageService, string message) : base(messageService, message)
    {
    }
}
