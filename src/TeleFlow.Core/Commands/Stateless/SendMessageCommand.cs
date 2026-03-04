using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateless;

public delegate Task<InlineMarkupMessage> MessageFactory(IServiceProvider sp);

public class SendMessageCommand : StatelessCommandBase
{
    private readonly MessageFactory _message;

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

    public SendMessageCommand(MessageFactory message)
    {
        _message = message;
    }
}