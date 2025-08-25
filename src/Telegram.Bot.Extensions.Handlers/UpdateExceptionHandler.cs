using System.Text;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram;

public class UpdateExceptionHandler : IHandler<Update>
{
    private readonly IHandler<Update> _next;

    private readonly Func<Exception, Task> _exceptionHandler;


    public async Task Handle(Update args)
    {
        try
        {
            await _next.Handle(args);
        }
        catch(Exception ex)
        {
            await _exceptionHandler.Invoke(ex);
        }
    }

    public UpdateExceptionHandler(IHandler<Update> next, Func<Exception, Task> exceptionHandler)
    {
        _next = next;
        _exceptionHandler = exceptionHandler;
    }
}
