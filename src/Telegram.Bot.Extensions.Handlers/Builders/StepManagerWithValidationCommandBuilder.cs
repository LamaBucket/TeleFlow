using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class StepManagerWithValidationCommandBuilder<TState>
{
    private readonly Queue<Func<MultiStepCommandBuilderArgs<TState>, ICommandFactory<StepCommand, Update, StepCommand>>> _factoryBuildQueue;


    public StepManagerWithValidationCommandBuilder<TState> WithStepWithValidation(Func<MultiStepCommandBuilderArgs<TState>, ICommandFactory<StepCommand, Update, StepCommand>> stepFactory, string stepValidationDisplayName)
    {
        return WithStep((args) => {
            return new StepCommandWithValidationFactory(stepFactory.Invoke(args), () => { return stepValidationDisplayName;});
        });
    }

    public StepManagerWithValidationCommandBuilder<TState> WithStep(Func<MultiStepCommandBuilderArgs<TState>, ICommandFactory<StepCommand, Update, StepCommand>> stepFactory)
    {
        _factoryBuildQueue.Enqueue(stepFactory);

        return this;
    }

    public Tuple<StepManagerCommand, IEnumerable<Tuple<IStateValidationDisplayNameProvider, ICommandFactory<StepCommand, Update, StepCommand>>>> Build(MultiStepCommandBuilderArgs<TState> args)
    {
        var stepChainBuilder = args.StepChainBuilder;

        Queue<Func<MultiStepCommandBuilderArgs<TState>, ICommandFactory<StepCommand, Update, StepCommand>>> factoryBuildQueueClone = new(_factoryBuildQueue);
    
        List<Tuple<IStateValidationDisplayNameProvider, ICommandFactory<StepCommand, Update, StepCommand>>> factoriesWithValidation = new();

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
