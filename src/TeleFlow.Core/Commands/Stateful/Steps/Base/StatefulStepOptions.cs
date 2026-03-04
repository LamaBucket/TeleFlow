using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.Base;

public class StatefulStepOptions<TData>
    where TData : StepViewModel
{
    public required IStepRenderService<TData> RenderService { get; init; }

    public string StateExpiredMessage { get; init; } = "Your State Expired.";
    
}