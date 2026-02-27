using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Abstractions.Engine.Commands.Filters;

public delegate Task<CommandStepResult> StepHandleDelegate(UpdateContext context);

public delegate Task StepEnterDelegate(IServiceProvider serviceProvider);


public interface ICommandStepFilter
{
    Task<CommandStepResult> ExecuteOnUpdate(UpdateContext context, StepHandleDelegate next);

    Task ExecuteOnEnter(IServiceProvider serviceProvider, StepEnterDelegate next);
}