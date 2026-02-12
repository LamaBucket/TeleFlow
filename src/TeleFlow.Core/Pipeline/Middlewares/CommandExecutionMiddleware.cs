using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands;
using TeleFlow.Commands.Factories;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Pipeline.Middlewares;

public class CommandExecutionMiddleware : IHandlerMiddleware<SessionContext, CommandResultContext>
{
    public IHandler<CommandResultContext> Next { get; init; }

    private readonly ICommandFactory<ICommandHandler, ChatSession> _commandFactory;

    public async Task Handle(SessionContext args)
    {
        var command = _commandFactory.Create(args.Session);

        var commandResult = await command.Handle(args);

        CommandResultContext context = new(commandResult, args);

        await Next.Handle(context);
    }

    public CommandExecutionMiddleware(IHandler<CommandResultContext> next, ICommandFactory<ICommandHandler, ChatSession> commandFactory)
    {
        Next = next;
        _commandFactory = commandFactory;
    }
}