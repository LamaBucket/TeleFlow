using Telegram.Bot.Extensions.Handlers.Commands;
using Telegram.Bot.Extensions.Handlers.Commands.MultiStep;
using Telegram.Bot.Extensions.Handlers.Factories;
using Telegram.Bot.Extensions.Handlers.Models;
using Telegram.Bot.Extensions.Handlers.Models;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Builders;

public class MultiStepCommandBuilder<TState, TBuildArgs> where TBuildArgs : class
{
    private Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactory<ICommandHandler, Update>>? _nextFactory;

    private Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, IHandlerFactory<IHandler<TState>, TState>>? _resultHandlerFactory;

    private Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, State<TState>>? _state;


    public MultiStepCommandBuilder<TState, TBuildArgs> SetDefaultStateValueBasedOnArgs(Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, TState> value)
    {
        _state = (args) => { return new(value.Invoke(args)); };

        return this;
    }

    public MultiStepCommandBuilder<TState, TBuildArgs> SetDefaultStateValue(TState value)
    {
        _state = (args) => { return new(value); };

        return this;
    }


    public MultiStepCommandBuilder<TState, TBuildArgs> WithValidation(MessageBuilderOptions messageBuilderConfig,
                                                                                            string allGoodButtonDisplayName,
                                                                                            string editButtonDisplayName,
                                                                                            Func<TState, string> confirmMessageFormatter,
                                                                                            Action<StepManagerWithValidationCommandBuilder<TState, TBuildArgs>> options)
    {
        if(_nextFactory is not null)
            throw new InvalidOperationException($"The {nameof(_nextFactory)} is already set!");

        _nextFactory = (args) => {
            var buildTimeArgs = args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs;
            
            if (buildTimeArgs is IArgsWithMessageService msgServiceArgs)
            {
                StepManagerWithValidationCommandBuilder<TState, TBuildArgs> builder = new();
                options.Invoke(builder);

                var stepManagerBuildResult = builder.Build(args);

                var stepManager = stepManagerBuildResult.Item1;
                var stepsWithValidation = stepManagerBuildResult.Item2;


                StateValidationMessageFormatter<TState> formatter = new(messageBuilderConfig, confirmMessageFormatter, stepsWithValidation, allGoodButtonDisplayName);
                StateValidationMessageFormatterWithNoButtons<TState> formatterWithNoButtons = new(messageBuilderConfig, confirmMessageFormatter, allGoodButtonDisplayName, editButtonDisplayName);
                StateValidatorCommandFactory<TState> validatorFactory = new(args.StepChainBuilder, formatter, formatterWithNoButtons, msgServiceArgs.MessageService, args.State);

                validatorFactory.SetContext(stepManager);

                return new LambdaCommandFactory(validatorFactory.Create);
            }

            throw new InvalidCastException("To use validation ");
        };

        return this;
    }


    public MultiStepCommandBuilder<TState, TBuildArgs> WithoutValidation(Action<StepManagerCommandBuilder<TState, TBuildArgs>> options)
    {
        if(_nextFactory is not null)
            throw new InvalidOperationException($"The {nameof(_nextFactory)} is already set!");

        _nextFactory = (args) => {
            StepManagerCommandBuilder<TState, TBuildArgs> builder = new();

            options.Invoke(builder);
            
            return new LambdaCommandFactory<MultiStepCommandBuilderArgs<TState, TBuildArgs>>(builder.Build, args);
        };

        return this;
    }


    public MultiStepCommandBuilder<TState, TBuildArgs> WithLambdaResult(Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, IHandler<TState>> handlerFactory)
    {
        _resultHandlerFactory = (args) => {
            return new LambdaHandlerFactory<IHandler<TState>, TState, UpdateListenerCommandExecutionArgs<TBuildArgs>>(handlerFactory, args.UpdateListenerBuilderArgs);
        };

        return this;
    }


    public StateCommand<TState> Build(UpdateListenerCommandExecutionArgs<TBuildArgs> args)
    {
        if(_nextFactory is null)
            throw new ArgumentNullException(nameof(_nextFactory));

        if(_resultHandlerFactory is null)
            throw new ArgumentNullException(nameof(_resultHandlerFactory));

        if(_state is null)
            throw new ArgumentNullException(nameof(_state));

        var state = _state.Invoke(args);

        MultiStepCommandBuilderArgs<TState, TBuildArgs> multiStepArgs = new(args, state, new StepChainBuilder());

        return new StateCommand<TState>(_nextFactory.Invoke(multiStepArgs).Create(), _resultHandlerFactory.Invoke(multiStepArgs).Create(), state);
    }


    internal MultiStepCommandBuilder()
    {
    }
}