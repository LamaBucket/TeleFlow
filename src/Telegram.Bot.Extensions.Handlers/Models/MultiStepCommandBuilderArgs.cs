using Telegram.Bot.Extensions.Handlers.Commands.MultiStep;

namespace Telegram.Bot.Extensions.Handlers.Models;

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