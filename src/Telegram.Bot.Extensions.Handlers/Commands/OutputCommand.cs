using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Commands;

public abstract class OutputCommand : ICommandHandler
{
    public event Func<Task>? CommandFinished;

    public async Task Handle(Update args)
    {
        throw new InvalidOperationException("The Handle Should not be executed in output commands");
    }

    public async Task OnCommandCreated()
    {
        await Handle();

        if(CommandFinished is not null)
            await CommandFinished.Invoke();
    }

    protected abstract Task Handle();

    protected OutputCommand()
    {
        
    }
}
