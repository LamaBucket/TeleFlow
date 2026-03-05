using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.Base;

public class StatefulStepOptions<TData>
    where TData : StepData
{
    public required IStepRenderService<TData> RenderService { get; init; }

    public string? StateExpiredMessage { get; init; }
    
}