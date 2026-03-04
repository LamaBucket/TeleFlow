using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public record TextInputStepData(string TextEntered) : StepData
{
    public static TextInputStepData Default
        => new(string.Empty);
}