using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class StepManagerCommandBuilder<TState>
{
    private readonly Queue<Func<MultiStepCommandBuilderArgs<TState>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> _factoryBuildQueue;


    public virtual StepManagerCommandBuilder<TState> WithStep(Func<MultiStepCommandBuilderArgs<TState>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> stepFactory)
    {
        _factoryBuildQueue.Enqueue(stepFactory);

        return this;
    }

    public StepManagerCommand Build(MultiStepCommandBuilderArgs<TState> args)
    {
        SetupStepChainBuilder(args);

        return new StepManagerCommand(args.StepChainBuilder);
    }

    private void SetupStepChainBuilder(MultiStepCommandBuilderArgs<TState> args)
    {
        Queue<Func<MultiStepCommandBuilderArgs<TState>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> factoryBuildQueueClone = new(_factoryBuildQueue);

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