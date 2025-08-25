using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class UpdateListenerCommandFactoryBuilder
{
    private readonly Dictionary<string, IHandlerFactoryWithArgs<ICommandHandler, Update, NavigatorFactoryArgs>> _factories;


    private readonly Queue<Action<UpdateListenerFactoryBuildArgs>> _oneTimeSetupActionQueue;

    private bool _oneTimeSetupComplete;


    private readonly Queue<Action<UpdateListenerFactoryBuildArgs>> _setupQueue;


    public UpdateListenerCommandFactoryBuilder WithAuthentication(Action<UpdateListenerCommandFactoryBuilder> options, Func<UpdateListenerCommandFactoryArgs, ICommandHandler> handlerIfNotAuthenticated)
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

                WithAuthentication(commandName, options =>
                {
                    options
                    .WithLambdaIfAuthenticated((args) =>
                    {
                        commandFactory.SetContext(args.NavigatorArgs);
                        return commandFactory.Create();
                    })
                    .WithLambdaIfNotAuthenticated(handlerIfNotAuthenticated);
                });
            }

            builder.ClearFactorySetup();
        });

        return this;
    }

    public UpdateListenerCommandFactoryBuilder WithAuthentication(string command, Action<AuthenticationCommandBuilder> options)
    {
        AuthenticationCommandBuilder builder = new();

        options.Invoke(builder);

        return WithCondition(command, options =>
        {
            options
            .WithCondition(async (args) =>
            {
                return await args.AuthenticationService.IsAuthenticated();
            })
            .WithLambdaIfTrue(builder.HandlerIfAuthenticated)
            .WithLambdaIfFalse(builder.HandlerIfNotAuthenticated);
        });
    }

    public UpdateListenerCommandFactoryBuilder WithCondition(string command, Action<ConditionalCommandBuilder> options)
    {
        ConditionalCommandBuilder builder = new();

        options.Invoke(builder);

        return WithLambda(command, builder.Build);
    }

    public UpdateListenerCommandFactoryBuilder WithSendText(string command, string text)
    {
        return WithLambda(command, (UpdateListenerCommandFactoryArgs args) => {
            return new SendTextCommand(args.MessageServiceString, text);
        });
    }

    public UpdateListenerCommandFactoryBuilder WithMultiStep<TState>(string command, Action<MultiStepCommandBuilder<TState>> options)
    {
        MultiStepCommandBuilder<TState> builder = new();

        options.Invoke(builder);

        return WithLambda(command, builder.Build);
    }

    
    public UpdateListenerCommandFactoryBuilder WithLambda(string command, Func<UpdateListenerCommandFactoryArgs, ICommandHandler> factory)
    {
        _setupQueue.Enqueue((UpdateListenerFactoryBuildArgs args) => {
            _factories.Add(command, new LambdaCommandFactoryWithArgs<NavigatorFactoryArgs>((navigatorArgs) => {

                var listenerCommandTimeArgs = new UpdateListenerCommandFactoryArgs(navigatorArgs, args.MessageServiceString, args.MessageService, args.MessageServiceWithReplyButton, args.ChatIdProvider, args.Navigator, args.ReplyMarkupManager, args.AuthenticationService);
                var listenerArgsFactory = new LambdaCommandFactory<UpdateListenerCommandFactoryArgs>(factory, listenerCommandTimeArgs);
                
                return listenerArgsFactory.Create();
            }));
        });

        return this;
    }

    public UniversalCommandFactory Build(UpdateListenerFactoryBuildArgs args)
    {
        UniversalCommandFactory result = new();

        SetupFactory(args);

        foreach(var factoryKvp in _factories)
        {
            result.AddCommandFactory(factoryKvp.Key, factoryKvp.Value);
        }

        ClearFactorySetup();

        return result;
    }

    private void SetupFactory(UpdateListenerFactoryBuildArgs args)
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