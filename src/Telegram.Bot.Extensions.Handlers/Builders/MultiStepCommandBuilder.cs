using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class MultiStepCommandBuilder<TState>
{
    private Func<MultiStepCommandBuilderArgs<TState>, IHandlerFactory<ICommandHandler, Update>>? _nextFactory;

    private Func<MultiStepCommandBuilderArgs<TState>, IHandlerFactory<IHandler<TState>, TState>>? _resultHandlerFactory;

    private Func<UpdateListenerCommandFactoryArgs, State<TState>>? _state;


    public MultiStepCommandBuilder<TState> SetDefaultStateValueBasedOnArgs(Func<UpdateListenerCommandFactoryArgs, TState> value)
    {
        _state = (args) => { return new(value.Invoke(args)); };

        return this;
    }

    public MultiStepCommandBuilder<TState> SetDefaultStateValue(TState value)
    {
        _state = (args) => { return new(value); };

        return this;
    }


    public MultiStepCommandBuilder<TState> WithValidation(MessageBuilderOptions messageBuilderConfig, Func<TState, string> confirmMessageFormatter, string allGoodButtonDisplayName, Action<StepManagerWithValidationCommandBuilder<TState>> options)
    {
        if(_nextFactory is not null)
            throw new Exception("The Factory is already set!");

        _nextFactory = (args) => {
            StepManagerWithValidationCommandBuilder<TState> builder = new();
            options.Invoke(builder);

            var stepManagerBuildResult = builder.Build(args);

            var stepManager = stepManagerBuildResult.Item1;
            var stepsWithValidation = stepManagerBuildResult.Item2;
            

            StateValidationMessageFormatter<TState> formatter = new(messageBuilderConfig, confirmMessageFormatter, stepsWithValidation, allGoodButtonDisplayName);
            StateValidatorCommandFactory<TState> validatorFactory = new(args.StepChainBuilder, formatter, args.UpdateListenerBuilderArgs.MessageService, args.State);
            
            validatorFactory.SetContext(stepManager);

            return new LambdaCommandFactory(validatorFactory.Create);
        };

        return this;
    }


    public MultiStepCommandBuilder<TState> WithoutValidation(Action<StepManagerCommandBuilder<TState>> options)
    {
        if(_nextFactory is not null)
            throw new Exception("The Factory is already set!");

        _nextFactory = (args) => {
            StepManagerCommandBuilder<TState> builder = new();

            options.Invoke(builder);
            
            return new LambdaCommandFactory<MultiStepCommandBuilderArgs<TState>>(builder.Build, args);
        };

        return this;
    }


    public MultiStepCommandBuilder<TState> WithLambdaResult(Func<UpdateListenerCommandFactoryArgs, IHandler<TState>> handlerFactory)
    {
        _resultHandlerFactory = (args) => {
            return new LambdaHandlerFactory<IHandler<TState>, TState, UpdateListenerCommandFactoryArgs>(handlerFactory, args.UpdateListenerBuilderArgs);
        };

        return this;
    }


    public StateCommand<TState> Build(UpdateListenerCommandFactoryArgs args)
    {
        if(_nextFactory is null)
            throw new Exception("Unable To build Command without next");
        
        if(_resultHandlerFactory is null)
            throw new Exception("Unable to build Command without result");

        if(_state is null)
            throw new Exception("Unable to build Command without default state value");

        var state = _state.Invoke(args);

        MultiStepCommandBuilderArgs<TState> multiStepArgs = new(args, state, new StepChainBuilder());

        return new StateCommand<TState>(_nextFactory.Invoke(multiStepArgs).Create(), _resultHandlerFactory.Invoke(multiStepArgs).Create(), state);
    }


    internal MultiStepCommandBuilder()
    {
    }
}