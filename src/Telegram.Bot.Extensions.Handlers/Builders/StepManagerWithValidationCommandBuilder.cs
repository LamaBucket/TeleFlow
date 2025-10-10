using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class StepManagerWithValidationCommandBuilder<TState, TBuildArgs> where TBuildArgs : class
{
    private readonly Queue<Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> _factoryBuildQueue;


    public StepManagerWithValidationCommandBuilder<TState, TBuildArgs> WithStepWithValidation(Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> stepFactory, string stepValidationDisplayName)
    {
        return WithStep((args) => {
            return new StepCommandWithValidationFactory(stepFactory.Invoke(args), () => { return stepValidationDisplayName;});
        });
    }

    public StepManagerWithValidationCommandBuilder<TState, TBuildArgs> WithStep(Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> stepFactory)
    {
        _factoryBuildQueue.Enqueue(stepFactory);

        return this;
    }

    public Tuple<StepManagerCommand, IEnumerable<Tuple<IStateValidationDisplayNameProvider, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>>> Build(MultiStepCommandBuilderArgs<TState, TBuildArgs> args)
    {
        var stepChainBuilder = args.StepChainBuilder;

        Queue<Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> factoryBuildQueueClone = new(_factoryBuildQueue);
    
        List<Tuple<IStateValidationDisplayNameProvider, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> factoriesWithValidation = new();

        while(factoryBuildQueueClone.Count != 0)
        {
            var current = factoryBuildQueueClone.Dequeue();

            var factory = current.Invoke(args);

            if(factory is IStateValidationDisplayNameProvider factoryWithDisplayName)
            {
                factoriesWithValidation.Add(new(factoryWithDisplayName, factory));
            }

            stepChainBuilder.AddItemToChain(factory);
        }

        return new(new StepManagerCommand(stepChainBuilder), factoriesWithValidation);
    }


    internal StepManagerWithValidationCommandBuilder()
    {
        _factoryBuildQueue = new();
    }
}
