using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Commands.Flow;

public interface IFlowStep
{
    Task<FlowStepResult> Handle(UpdateContext args);

    Task OnEnter(IServiceProvider serviceProvider);
}