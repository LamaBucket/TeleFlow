using Telegram.Bot.Extensions.Handlers.Services.Messaging;

namespace LisBot.Common.Telegram.Commands;

public class SendMessageCommand<TMessage> : OutputCommand
{
    private readonly IMessageService<TMessage> _messageService;

    private readonly TMessage _message;

    protected override async Task Handle()
    {
        await _messageService.SendMessage(_message);
    }

    public SendMessageCommand(IMessageService<TMessage> messageService, TMessage message)
    {
        _messageService = messageService;
        _message = message;
    }
}