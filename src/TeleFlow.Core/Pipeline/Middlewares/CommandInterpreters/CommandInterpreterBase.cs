using TeleFlow.Commands.Results;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Pipeline.Middlewares.CommandInterpreters;

public abstract class CommandInterpreterBase<TCommandResult> : IHandlerMiddleware<CommandResultContext> where TCommandResult : CommandResult
{
    public IHandler<CommandResultContext> Next { get; init; }

    public async Task Handle(CommandResultContext args)
    {
        if(args.CommandResult is TCommandResult typedResult)
        {
            CommandResultContext<TCommandResult> typedContext = new(typedResult, args);
            
            await Handle(typedContext);

            if (ContinueAfterMatch)
            {
                await Next.Handle(args);
            }
        }
        else
        {
            await Next.Handle(args);
        }
    }

    protected abstract bool ContinueAfterMatch { get; }

    protected abstract Task Handle(CommandResultContext<TCommandResult> args);


    public CommandInterpreterBase(IHandler<CommandResultContext> next)
    {
        Next = next;
    }
}