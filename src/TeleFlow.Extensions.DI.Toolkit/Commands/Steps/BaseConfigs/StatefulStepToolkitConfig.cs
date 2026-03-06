using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.BaseConfigs;

public abstract class StatefulStepToolkitConfig
{
    public string? MessageStateExpired { get; set; } = StatefulStepDefaults.StateExpiredMessage;

    protected StatefulStepOptions<T> BuildStatefulOptions<T>(IStepRenderService<T> renderService) where T : StepData
        => new() { RenderService = renderService, StateExpiredMessage = MessageStateExpired };
}