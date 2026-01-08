using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Commands.Stateless;

public abstract class OutputCommand : ICommandHandler
{
    public event Func<Task>? CommandFinished;

    public async Task<CommandResult> Handle(UpdateContext update)
    {
        await ExecuteCommand(update);

        return CommandResult.Exit;
    }

    protected abstract Task ExecuteCommand(UpdateContext context);

    protected OutputCommand()
    {
        
    }
}
