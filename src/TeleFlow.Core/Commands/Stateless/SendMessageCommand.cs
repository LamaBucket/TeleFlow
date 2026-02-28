using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateless;

public class SendMessageCommand : StatelessCommandBase
{
    private readonly Func<IServiceProvider, Task<InlineMarkupMessage>> _message;

    protected override async Task ExecuteCommand(UpdateContext context)
    {
        var messageService = context.GetService<IMessageSendService>();

        var message = await _message.Invoke(context.ServiceProvider);
        await messageService.SendMessage(message);
    }


    public SendMessageCommand(string message) : this(InlineMarkupMessage.CreateTextMessage(message))
    {
    }

    public SendMessageCommand(InlineMarkupMessage message) : this(async () => message )
    {
    }

    public SendMessageCommand(Func<Task<InlineMarkupMessage>> message) : this((sp) => message.Invoke())
    {
    }

    public SendMessageCommand(Func<IServiceProvider, Task<InlineMarkupMessage>> message)
    {
        _message = message;
    }
}