using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Abstractions.Engine.Commands.Interceptors;

public interface ICommandStepInterceptor
{
    Task<CommandStepResult?> InterceptBeforeStep(UpdateContext context);

    Task<CommandStepResult>  InterceptAfterStep(UpdateContext context, CommandStepResult stepResult);


    Task InterceptBeforeOnEnter(IServiceProvider serviceProvider);

    Task InterceptAfterOnEnter(IServiceProvider serviceProvider);
}