namespace TeleFlow.Commands.Results;

public abstract class CommandResult
{
    public static CommandResult Exit => new ExitCommandResult();
}