using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.Base;

public class CallbackStepOptions<TData>
    where TData : StepData
{
    public required StatefulStepOptions<TData> RenderConfig { get; init; }

    public string? NoCallbackQueryMessage { get; init; }
    public string? UnknownCallbackQueryActionMessage { get; init; }
    public string? CallbackQueryExpiredMessage { get; init; }
}