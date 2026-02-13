using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Factories;

namespace TeleFlow.Core.Pipeline;

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