namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public class TextInputCommandStepOptions
{
    public required string UserPrompt { get; init; }

    public required Func<CommandStepCommitContext, string, Task> OnUserCommit { get; set; }

    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";

    public string NoTextProvidedMessage { get; init; } = "This command accepts only text";
}