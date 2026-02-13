using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Abstractions.Engine.Commands.Interceptors;

public interface ICommandInterceptor
{
    Task<CommandResult?> Intercept(UpdateContext update);
}