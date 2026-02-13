using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateless;

public abstract class StatelessCommandBase : ICommandHandler
{
    public async Task<CommandResult> Handle(UpdateContext update)
    {
        await ExecuteCommand(update);

        return CommandResult.Exit;
    }

    protected abstract Task ExecuteCommand(UpdateContext context);

    protected StatelessCommandBase()
    {
        
    }
}
