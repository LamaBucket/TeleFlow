using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;

public record SingleInputStepData<TInput>(TInput? Input) : StepData
{
    internal static SingleInputStepData<TInput> CreateDefault()
    {
        return new SingleInputStepData<TInput>(Input: default);
    }
};