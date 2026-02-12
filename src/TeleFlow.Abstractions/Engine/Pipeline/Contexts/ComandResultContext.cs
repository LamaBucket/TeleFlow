using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Results;

namespace TeleFlow.Pipeline.Contexts;

public class CommandResultContext<TCommandResult> : SessionContext where TCommandResult : CommandResult
{
    public TCommandResult CommandResult { get; init; }


    public CommandResultContext(TCommandResult commandResult, SessionContext sessionContext) : this(commandResult, sessionContext.Session, sessionContext)
    {
        
    }

    public CommandResultContext(TCommandResult commandResult, ChatSession session, UpdateContext context) : base(session, context)
    {
        CommandResult = commandResult;
    }
}

public class CommandResultContext : CommandResultContext<CommandResult>
{
    public CommandResultContext(CommandResult commandResult, SessionContext sessionContext) : base(commandResult, sessionContext)
    {
    }
}