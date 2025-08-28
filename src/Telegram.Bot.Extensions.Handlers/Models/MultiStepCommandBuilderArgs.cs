using LisBot.Common.Telegram.Commands.MultiStep;

namespace LisBot.Common.Telegram.Models;

public class MultiStepCommandBuilderArgs<TState>
{
    public UpdateListenerCommandExecutionArgs UpdateListenerBuilderArgs { get; init; }

    public State<TState> State { get; init; }

    public StepChainBuilder StepChainBuilder { get; init; }

    public MultiStepCommandBuilderArgs(UpdateListenerCommandExecutionArgs updateListenerBuilderArgs, State<TState> state, StepChainBuilder stepChainBuilder)
    {
        UpdateListenerBuilderArgs = updateListenerBuilderArgs;
        State = state;
        StepChainBuilder = stepChainBuilder;
    }
}