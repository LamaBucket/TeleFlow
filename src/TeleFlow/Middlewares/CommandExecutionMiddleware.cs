using TeleFlow.Commands;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Models.Contexts;

namespace TeleFlow.Middlewares;

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