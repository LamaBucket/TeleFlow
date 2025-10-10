using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class StepManagerCommandBuilder<TState, TBuildArgs> where TBuildArgs : class
{
    private readonly Queue<Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> _factoryBuildQueue;


    public virtual StepManagerCommandBuilder<TState, TBuildArgs> WithStep(Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> stepFactory)
    {
        _factoryBuildQueue.Enqueue(stepFactory);

        return this;
    }

    public StepManagerCommand Build(MultiStepCommandBuilderArgs<TState, TBuildArgs> args)
    {
        SetupStepChainBuilder(args);

        return new StepManagerCommand(args.StepChainBuilder);
    }

    private void SetupStepChainBuilder(MultiStepCommandBuilderArgs<TState, TBuildArgs> args)
    {
        Queue<Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> factoryBuildQueueClone = new(_factoryBuildQueue);

        while(factoryBuildQueueClone.Count != 0)
        {
            var current = factoryBuildQueueClone.Dequeue();

            args.StepChainBuilder.AddItemToChain(current.Invoke(args));
        }
    }

    internal StepManagerCommandBuilder()
    {
        _factoryBuildQueue = new();
    }
}