using TeleFlow.Abstractions.Messaging;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Commands.Instant;

public class SendMessageCommand : InstantCommand
{
    private readonly Func<Task<OutgoingMessage>> _message;

    protected override async Task ExecuteCommand(UpdateContext context)
    {
        var messageService = context.GetService<IMessageSender>();

        var message = await _message.Invoke();
        await messageService.SendMessage(message);
    }


    public SendMessageCommand(string message) : this(OutgoingMessage.CreateTextMessage(message))
    {
    }

    public SendMessageCommand(OutgoingMessage message) : this(async () => message )
    {
    }

    public SendMessageCommand(Func<Task<OutgoingMessage>> message)
    {
        _message = message;
    }
}