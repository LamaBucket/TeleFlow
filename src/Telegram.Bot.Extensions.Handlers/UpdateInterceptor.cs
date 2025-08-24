
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram;

public class UpdateInterceptor : IHandler<Update>
{
    private readonly UpdateListener _listener;

    private readonly string[] _commandsToIntercept;

    public async Task Handle(Update args)
    {
        var commandName = args.GetCommandName();

        if(commandName is not null && _commandsToIntercept.Contains(commandName))
        {
            await _listener.Handle(commandName);
        }
        else
        {
            await _listener.Handle(args);
        }
    }

    public UpdateInterceptor(UpdateListener listener, string[] commandsToIntercept)
    {
        _listener = listener;
        _commandsToIntercept = commandsToIntercept;
    }
}
