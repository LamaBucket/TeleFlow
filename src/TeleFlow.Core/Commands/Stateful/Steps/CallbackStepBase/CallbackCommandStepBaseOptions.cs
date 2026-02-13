namespace TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;

public class CallbackCommandStepBaseOptions
{
    public required string UserPrompt { get; init; }


    public string NoCallbackQueryMessage { get; init; } = "Use the buttons linked to the previous message";

    public string UnknownCallbackQueryActionMessage { get; init; } = "The query contains an unknown action";

    public string CallbackQueryExpiredMessage { get; init; } = "This button's query is expired";
}