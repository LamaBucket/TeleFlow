using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.TextInput;

public record TextInputStepOptions
{
    public required StatefulStepOptions<SingleInputStepData<string>> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, string, Task> OnUserCommit { get; init; }

    public string? NoMessageInputMessage { get; init; }

    public string? NoTextProvidedMessage { get; init; }
}