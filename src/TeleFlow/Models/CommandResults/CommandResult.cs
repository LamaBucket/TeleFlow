using System.Reflection.Metadata.Ecma335;
using TeleFlow.Models.MultiStep;

namespace TeleFlow.Models.CommandResults;

public abstract class CommandResult
{
    public static CommandResult Exit => new ExitCommandResult();
}