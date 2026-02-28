using TeleFlow.Core.Commands.Stateful.Steps.ViewModelStepBase;

namespace TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;

public class CallbackCommandStepBaseOptions<TViewModel>
    where TViewModel : class
{
    public required ViewModelCommandStepBaseOptions<TViewModel> BaseOptions { get; init; }

    public string NoCallbackQueryMessage { get; init; } = "Use the buttons linked to the previous message";

    public string UnknownCallbackQueryActionMessage { get; init; } = "The query contains an unknown action";

    public string CallbackQueryExpiredMessage { get; init; } = "This button's query is expired";
}