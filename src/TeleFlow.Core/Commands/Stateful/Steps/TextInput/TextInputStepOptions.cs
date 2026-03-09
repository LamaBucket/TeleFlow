using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public record TextInputStepOptions
{
    public required StatefulStepOptions<TextInputStepData> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, string, Task> OnUserCommit { get; init; }

    public string? NoMessageInputMessage { get; init; }

    public string? NoTextProvidedMessage { get; init; }
}