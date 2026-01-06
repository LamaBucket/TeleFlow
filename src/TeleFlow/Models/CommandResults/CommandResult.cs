using System.Reflection.Metadata.Ecma335;

namespace TeleFlow.Models.CommandResults;

public abstract class CommandResult
{
    public static CommandResult Continue => new ContinueCommandResult();

    public static CommandResult Exit => new ExitCommandResult();
}