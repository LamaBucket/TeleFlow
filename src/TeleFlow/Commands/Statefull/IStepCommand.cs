using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;

namespace TeleFlow.Commands.Statefull;

public interface IStepCommand
{
    Task<StepResult> Handle(UpdateContext args);

    Task OnEnter();
}