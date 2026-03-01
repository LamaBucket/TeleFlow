using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;

public class StepBaseOptions<TViewModel>
    where TViewModel : StepViewModel
{
    public required IStepRenderService<TViewModel> RenderService { get; init; }

    public string StateExpiredMessage { get; init; } = "Your State Expired.";
    
}