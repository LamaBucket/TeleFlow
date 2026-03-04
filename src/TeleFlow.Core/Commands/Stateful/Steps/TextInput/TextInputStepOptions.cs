using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public class TextInputCommandStepOptions
{
    public required StatefulStepOptions<TextInputCommandStepViewModel> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, string, Task> OnUserCommit { get; init; }

    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";

    public string NoTextProvidedMessage { get; init; } = "This command accepts only text";
}