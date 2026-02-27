using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Abstractions.Engine.Commands.Filters;

public interface ICommandFilter
{
    Task<CommandResult> Execute(UpdateContext context, ICommandHandler next);
}