using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Commands.Stateless;

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
