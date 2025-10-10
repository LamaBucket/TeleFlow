using LisBot.Common.Telegram.Commands.MultiStep;

namespace LisBot.Common.Telegram.Models;

public class MultiStepCommandBuilderArgs<TState, TBuildArgs> where TBuildArgs : class
{
    public UpdateListenerCommandExecutionArgs<TBuildArgs> UpdateListenerBuilderArgs { get; init; }

    public State<TState> State { get; init; }

    public StepChainBuilder StepChainBuilder { get; init; }

    public MultiStepCommandBuilderArgs(UpdateListenerCommandExecutionArgs<TBuildArgs> updateListenerBuilderArgs, State<TState> state, StepChainBuilder stepChainBuilder)
    {
        UpdateListenerBuilderArgs = updateListenerBuilderArgs;
        State = state;
        StepChainBuilder = stepChainBuilder;
    }
}