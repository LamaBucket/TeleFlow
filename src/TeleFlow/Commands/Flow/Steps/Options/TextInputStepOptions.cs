namespace TeleFlow.Commands.Flow.Steps.Options;

public class TextInputStepOptions
{
    public required string UserPrompt { get; init; }

    public required Func<FlowStepCommitContext, string, Task> OnUserCommit { get; set; }

    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";

    public string NoTextProvidedMessage { get; init; } = "This command accepts only text";
}