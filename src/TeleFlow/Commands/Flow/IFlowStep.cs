using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;

namespace TeleFlow.Commands.Statefull;

public interface IFlowStep
{
    Task<FlowStepResult> Handle(UpdateContext args);

    Task OnEnter(IServiceProvider serviceProvider);
}