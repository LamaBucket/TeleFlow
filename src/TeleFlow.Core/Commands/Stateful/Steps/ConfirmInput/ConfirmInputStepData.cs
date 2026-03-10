using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput;

public record ConfirmInputStepData(bool? ValueSelected) : StepData
{
    public static ConfirmInputStepData CreateDefault() 
        => new ConfirmInputStepData(ValueSelected: null);
}