using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class UpdateListenerCommandFactoryBuilder<TBuildArgs> where TBuildArgs : class
{
    private readonly Dictionary<string, IHandlerFactoryWithArgs<ICommandHandler, Update, NavigatorFactoryArgs>> _factories;


    private readonly Queue<Action<UpdateListenerCommandBuildArgs<TBuildArgs>>> _oneTimeSetupActionQueue;

    private bool _oneTimeSetupComplete;


    private readonly Queue<Action<UpdateListenerCommandBuildArgs<TBuildArgs>>> _setupQueue;


    public UpdateListenerCommandFactoryBuilder<TBuildArgs> WithCondition(Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, Task<bool>> condition, Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, ICommandHandler> handlerIfNotAuthenticated, Action<UpdateListenerCommandFactoryBuilder<TBuildArgs>> options)
    {
        UpdateListenerCommandFactoryBuilder<TBuildArgs> builder = new();

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

    public UpdateListenerCommandFactoryBuilder<TBuildArgs> WithCondition(string command, Action<ConditionalCommandBuilder<TBuildArgs>> options)
    {
        ConditionalCommandBuilder<TBuildArgs> builder = new();

        options.Invoke(builder);

        return WithLambda(command, builder.Build);
    }


    public UpdateListenerCommandFactoryBuilder<TBuildArgs> WithMultiStep<TState>(string command, Action<MultiStepCommandBuilder<TState, TBuildArgs>> options)
    {
        MultiStepCommandBuilder<TState, TBuildArgs> builder = new();

        options.Invoke(builder);

        return WithLambda(command, builder.Build);
    }
    
    
    public UpdateListenerCommandFactoryBuilder<TBuildArgs> WithLambda(string command, Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, ICommandHandler> factory)
    {
        _setupQueue.Enqueue((UpdateListenerCommandBuildArgs<TBuildArgs> args) =>
        {
            _factories.Add(command, new LambdaCommandFactoryWithArgs<NavigatorFactoryArgs>((navigatorArgs) =>
            {

                var listenerCommandTimeArgs = new UpdateListenerCommandExecutionArgs<TBuildArgs>(navigatorArgs, args);
                var listenerArgsFactory = new LambdaCommandFactory<UpdateListenerCommandExecutionArgs<TBuildArgs>>(factory, listenerCommandTimeArgs);

                return listenerArgsFactory.Create();
            }));
        });

        return this;
    }


    public UniversalCommandFactory Build(UpdateListenerCommandBuildArgs<TBuildArgs> args)
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

    private void SetupFactory(UpdateListenerCommandBuildArgs<TBuildArgs> args)
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