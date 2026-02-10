using TeleFlow.Commands.Results;
using TeleFlow.Pipeline.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Commands.Instant;

public abstract class InstantCommand : ICommandHandler
{
    public async Task<CommandResult> Handle(UpdateContext update)
    {
        await ExecuteCommand(update);

        return CommandResult.Exit;
    }

    protected abstract Task ExecuteCommand(UpdateContext context);

    protected InstantCommand()
    {
        
    }
}
