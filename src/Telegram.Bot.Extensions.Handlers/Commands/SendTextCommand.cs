using LisBot.Common.Telegram.Services;

namespace LisBot.Common.Telegram.Commands;

public class SendTextCommand : OutputCommand
{
    private readonly IMessageService<string> _messageService;

    private readonly string _message;

    protected override async Task Handle()
    {
        await _messageService.SendMessage(_message);
    }

    public SendTextCommand(IMessageService<string> messageService, string message)
    {
        _messageService = messageService;
        _message = message;
    }
}
