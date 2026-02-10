using TeleFlow.Commands.Results;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Commands.Interceptors;

public interface ICommandInterceptor
{
    Task<CommandResult?> Intercept(UpdateContext update);
}