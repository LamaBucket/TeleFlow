using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;

public abstract class SingleInputStep<TInput>(StatefulStepOptions<SingleInputStepData<TInput>> options)
    : SingleInputStep<SingleInputStepData<TInput>, TInput>(options);

public abstract class SingleInputStep<TData, TInput>(StatefulStepOptions<TData> options) : StatefulStep<TData>(options)
    where TData : SingleInputStepData<TInput>
{
    protected async Task SetInputAndRerender(IServiceProvider sp, StepState<TData> state, TInput value)
    {
        state = state with { StepData = state.StepData with { Input = value } };
    
        await UpsertAndRerender(sp, state);    
    }
}