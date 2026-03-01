using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInputCommandStep;

public record TextInputCommandStepViewModel(string TextEntered) : StepViewModel
{
    public static TextInputCommandStepViewModel Default
        => new(string.Empty);
}