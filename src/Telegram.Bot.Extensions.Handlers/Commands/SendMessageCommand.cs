using Telegram.Bot.Extensions.Handlers.Services.Messaging;

namespace LisBot.Common.Telegram.Commands;

public class SendMessageCommand<TMessage> : OutputCommand
{
    private readonly IMessageService<TMessage> _messageService;

    private readonly Func<Task<TMessage>> _message;

    protected override async Task Handle()
    {
        var message = await _message.Invoke();
        await _messageService.SendMessage(message);
    }

    public SendMessageCommand(IMessageService<TMessage> messageService, TMessage message)
    {
        _messageService = messageService;
        _message = async () => { return message; };
    }

    public SendMessageCommand(IMessageService<TMessage> messageService, Func<Task<TMessage>> message)
    {
        _messageService = messageService;
        _message = message;
    }
}