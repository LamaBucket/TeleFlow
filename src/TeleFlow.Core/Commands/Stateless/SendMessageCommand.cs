using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateless;

public class SendMessageCommand : StatelessCommandBase
{
    private readonly Func<IServiceProvider, Task<OutgoingMessage>> _message;

    protected override async Task ExecuteCommand(UpdateContext context)
    {
        var messageService = context.GetService<IMessageSender>();

        var message = await _message.Invoke(context.ServiceProvider);
        await messageService.SendMessage(message);
    }


    public SendMessageCommand(string message) : this(OutgoingMessage.CreateTextMessage(message))
    {
    }

    public SendMessageCommand(OutgoingMessage message) : this(async () => message )
    {
    }

    public SendMessageCommand(Func<Task<OutgoingMessage>> message) : this((sp) => message.Invoke())
    {
    }

    public SendMessageCommand(Func<IServiceProvider, Task<OutgoingMessage>> message)
    {
        _message = message;
    }
}