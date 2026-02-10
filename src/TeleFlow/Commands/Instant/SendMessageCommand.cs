using TeleFlow.Models.Contexts;
using TeleFlow.Models.Messaging;
using TeleFlow.Services.Messaging;

namespace TeleFlow.Commands.Stateless;

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