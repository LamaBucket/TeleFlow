using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;

namespace TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase;

public class CallbackStepBaseOptions<TViewModel>
    where TViewModel : class
{
    public required StepBaseOptions<TViewModel> RenderConfig { get; init; }

    public string NoCallbackQueryMessage { get; init; } = "Use the buttons linked to the previous message";

    public string UnknownCallbackQueryActionMessage { get; init; } = "The query contains an unknown action";

    public string CallbackQueryExpiredMessage { get; init; } = "This button's query is expired";
}