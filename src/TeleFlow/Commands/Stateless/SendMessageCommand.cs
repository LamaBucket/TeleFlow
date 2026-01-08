using TeleFlow.Models.Contexts;
using TeleFlow.Services.Messaging;

namespace TeleFlow.Commands.Stateless;

public class SendMessageCommand<TMessage> : OutputCommand
{
    private readonly Func<Task<TMessage>> _message;

    protected override async Task ExecuteCommand(UpdateContext context)
    {
        var messageService = context.GetService<IMessageService<TMessage>>();

        var message = await _message.Invoke();
        await messageService.SendMessage(message);
    }

    public SendMessageCommand(TMessage message)
    {
        _message = async () => { return message; };
    }

    public SendMessageCommand(Func<Task<TMessage>> message)
    {
        _message = message;
    }
}