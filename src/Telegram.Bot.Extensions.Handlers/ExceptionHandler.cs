using System.Text;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram;

public class ExceptionHandler : IHandler<Update>
{
    private readonly IHandler<Update> _next;

    private readonly IMessageService<string> _messageService;
    
    private readonly INavigatorHandler _navigator;

    private readonly string baseCommand;


    public async Task Handle(Update args)
    {
        try
        {
            await _next.Handle(args);
        }
        catch(Exception ex)
        {
            StringBuilder sb = new();
            sb.AppendLine(ex.Message);

            await _messageService.SendMessage(sb.ToString());

            await _navigator.Handle(baseCommand);
        }
    }

    public ExceptionHandler(IHandler<Update> next, IMessageService<string> messageService, INavigatorHandler navigator, string baseCommand)
    {
        _next = next;
        _messageService = messageService;
        _navigator = navigator;
        this.baseCommand = baseCommand;
    }
}
