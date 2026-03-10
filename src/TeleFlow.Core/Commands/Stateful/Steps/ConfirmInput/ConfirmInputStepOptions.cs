using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput;

public record ConfirmInputStepOptions
{
    public required CallbackStepOptions<ConfirmInputStepData> CallbackStepOptions { get; init; }

    public required Func<CommandStepCommitContext, bool, Task<CommandStepResult>> OnCommit { get; init; }

    public required string? InvalidButtonMessage { get; init; }
}