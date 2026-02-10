using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;

namespace TeleFlow.Interceptors;

public interface ICommandInterceptor
{
    Task<CommandResult?> Intercept(UpdateContext update);
}