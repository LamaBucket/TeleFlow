using TeleFlow.Services.Messaging;

namespace TeleFlow.Commands.Stateless;

public class SendTextCommand : SendMessageCommand<string>
{
    public SendTextCommand(string message) : base(message)
    {
    }
}
