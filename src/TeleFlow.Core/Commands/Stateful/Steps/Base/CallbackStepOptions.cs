using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.Base;

public class CallbackStepOptions<TData>
    where TData : StepViewModel
{
    public required StatefulStepOptions<TData> RenderConfig { get; init; }

    public string NoCallbackQueryMessage { get; init; } = "Use the buttons linked to the previous message";

    public string UnknownCallbackQueryActionMessage { get; init; } = "The query contains an unknown action";

    public string CallbackQueryExpiredMessage { get; init; } = "This button's query is expired";
}