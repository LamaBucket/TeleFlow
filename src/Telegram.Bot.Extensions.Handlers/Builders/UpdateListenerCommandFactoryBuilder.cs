using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class UpdateListenerCommandFactoryBuilder
{
    private readonly Dictionary<string, IHandlerFactoryWithArgs<ICommandHandler, Update, NavigatorFactoryArgs>> _factories;


    private readonly Queue<Action<UpdateListenerCommandBuildArgs>> _oneTimeSetupActionQueue;

    private bool _oneTimeSetupComplete;


    private readonly Queue<Action<UpdateListenerCommandBuildArgs>> _setupQueue;


    public UpdateListenerCommandFactoryBuilder WithCondition(Func<UpdateListenerCommandExecutionArgs, Task<bool>> condition, Func<UpdateListenerCommandExecutionArgs, ICommandHandler> handlerIfNotAuthenticated, Action<UpdateListenerCommandFactoryBuilder> options)
    {
        UpdateListenerCommandFactoryBuilder builder = new();

        options.Invoke(builder);

        _oneTimeSetupActionQueue.Enqueue((args) =>
        {
            builder.SetupFactory(args);

            foreach (var kvp in builder._factories)
            {
                var commandName = kvp.Key;
                var commandFactory = kvp.Value;

                WithCondition(commandName, options =>
                {
                    options
                    .WithCondition(condition)
                    .WithLambdaIfTrue((args) =>
                    {
                        commandFactory.SetContext(args.NavigatorArgs);
                        return commandFactory.Create();
                    })
                    .WithLambdaIfFalse(handlerIfNotAuthenticated);
                });
            }

            builder.ClearFactorySetup();
        });

        return this;
    }

    public UpdateListenerCommandFactoryBuilder WithCondition(string command, Action<ConditionalCommandBuilder> options)
    {
        ConditionalCommandBuilder builder = new();

        options.Invoke(builder);

        return WithLambda(command, builder.Build);
    }


    public UpdateListenerCommandFactoryBuilder WithMultiStep<TState>(string command, Action<MultiStepCommandBuilder<TState>> options)
    {
        MultiStepCommandBuilder<TState> builder = new();

        options.Invoke(builder);

        return WithLambda(command, builder.Build);
    }


    public UpdateListenerCommandFactoryBuilder WithSendText(string command, string text)
    {
        return WithLambda(command, (UpdateListenerCommandExecutionArgs args) =>
        {
            return new SendTextCommand(args.MessageServiceString, text);
        });
    }
    
    
    public UpdateListenerCommandFactoryBuilder WithLambda(string command, Func<UpdateListenerCommandExecutionArgs, ICommandHandler> factory)
    {
        _setupQueue.Enqueue((UpdateListenerCommandBuildArgs args) =>
        {
            _factories.Add(command, new LambdaCommandFactoryWithArgs<NavigatorFactoryArgs>((navigatorArgs) =>
            {

                var listenerCommandTimeArgs = new UpdateListenerCommandExecutionArgs(navigatorArgs, args);
                var listenerArgsFactory = new LambdaCommandFactory<UpdateListenerCommandExecutionArgs>(factory, listenerCommandTimeArgs);

                return listenerArgsFactory.Create();
            }));
        });

        return this;
    }


    public UniversalCommandFactory Build(UpdateListenerCommandBuildArgs args)
    {
        UniversalCommandFactory result = new();

        SetupFactory(args);

        foreach (var factoryKvp in _factories)
        {
            result.AddCommandFactory(factoryKvp.Key, factoryKvp.Value);
        }

        ClearFactorySetup();

        return result;
    }

    private void SetupFactory(UpdateListenerCommandBuildArgs args)
    {
        if (!_oneTimeSetupComplete)
        {
            foreach (var beforeFactorySetupActions in _oneTimeSetupActionQueue)
            {
                beforeFactorySetupActions.Invoke(args);
            }

            _oneTimeSetupComplete = true;
        }

        foreach (var setupQueueItem in _setupQueue)
        {
            setupQueueItem.Invoke(args);
        }
    }

    private void ClearFactorySetup()
    {
        _factories.Clear();
    }


    public UpdateListenerCommandFactoryBuilder()
    {
        _factories = new();
        _oneTimeSetupActionQueue = new();
        _oneTimeSetupComplete = false;
        _setupQueue = new();
    }
}