
using Telegram.Bot.Types;

namespace TeleFlow;

public class UpdateInterceptor : IHandler<Update>
{
    private readonly UpdateListener _listener;

    private readonly string[] _commandsToIntercept;

    public async Task Handle(Update args)
    {
        var commandName = GetCommandName(args);

        if(commandName is not null && _commandsToIntercept.Contains(commandName))
        {
            await _listener.Handle(commandName);
        }
        else
        {
            await _listener.Handle(args);
        }
    }

    public virtual string? GetCommandName(Update args)
    {
        return args.GetCommandName();
    }

    public UpdateInterceptor(UpdateListener listener, string[] commandsToIntercept)
    {
        _listener = listener;
        _commandsToIntercept = commandsToIntercept;
    }
}
