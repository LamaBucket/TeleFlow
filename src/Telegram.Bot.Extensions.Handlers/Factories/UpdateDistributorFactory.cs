using System.ComponentModel;
using System.Diagnostics;
using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Factories;

public class UpdateDistributorFactory : IHandlerFactory<UpdateDistributor, Update>
{
    protected UpdateDistributor? Instance { get; set; }


    private readonly Func<IChatIdProvider, IMessageService<Message>> _messageServiceProvider;

    private readonly Func<IChatIdProvider, IMessageService<string>> _messageServiceStringProvider;

    private readonly Func<IChatIdProvider, IMessageService<Tuple<string, KeyboardButton>>> _messageServiceWithReplyKeyboardProvider;


    private readonly Func<IChatIdProvider, IReplyMarkupManager> _replyMarkupManagerProvider;

    private readonly Func<IChatIdProvider, IAuthenticationService> _authenticationServiceProvider;


    private Func<UpdateDistributorNextHandlerBuildArgs, IHandler<Update>, IHandler<Update>> _postBuildUpdateListenerSetup;


    public UpdateDistributor Create()
    {
        if (Instance is not null)
            return Instance;

        var updateDistributor = BuildUpdateDistributor();

        BuildFinished(updateDistributor);

        return updateDistributor;
    }

    private UpdateDistributor BuildUpdateDistributor()
    {
        var listenerFactory = BuildNextFactory();

        return new UpdateDistributor(listenerFactory);
    }

    protected IHandlerFactoryWithArgs<IHandler<Update>, Update, IChatIdProvider> BuildNextFactory()
    {
        return new LambdaHandlerFactoryWithArgs<IHandler<Update>, Update, IChatIdProvider>((chatIdProvider) =>
        {
            UpdateDistributorNextHandlerBuildArgs args = new(_messageServiceStringProvider.Invoke(chatIdProvider),
                                                             _messageServiceProvider.Invoke(chatIdProvider),
                                                             _messageServiceWithReplyKeyboardProvider.Invoke(chatIdProvider),
                                                             _replyMarkupManagerProvider.Invoke(chatIdProvider),
                                                             _authenticationServiceProvider.Invoke(chatIdProvider),
                                                             chatIdProvider);

            var listenerFactory = BuildUpdateListenerFactory();

            listenerFactory.SetContext(args);
            var listener = listenerFactory.Create();

            return _postBuildUpdateListenerSetup.Invoke(args, listener);
        });
    }

    protected UpdateListenerFactory BuildUpdateListenerFactory()
    {
        var listenerFactory = new UpdateListenerFactory((updateDistributorArgs, navHandler) =>
        {
            var updateListenerFactoryBuilder = new UpdateListenerCommandFactoryBuilder();

            SetupUpdateListenerFactoryBuilder(updateListenerFactoryBuilder);

            UpdateListenerCommandBuildArgs args = new(updateDistributorArgs, navHandler);

            return updateListenerFactoryBuilder.Build(args);
        });

        return listenerFactory;
    }

    protected virtual void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder builder)
    {

    }


    protected virtual void BuildFinished(UpdateDistributor buildResult)
    {
        Instance = buildResult;

        // if you need the update distributor not to be Singleton - clear the created instance here.
    }


    public UpdateDistributorFactory WithExceptionHandler(Func<Exception, UpdateDistributorNextHandlerBuildArgs, Task> handlerAction)
    {
        return WithCustomPostUpdateListenerBuildAction((args, handler) =>
        {
            UpdateExceptionHandler exceptionHandler = new(handler, async (ex) =>
            {
                await handlerAction.Invoke(ex, args);
            });

            return exceptionHandler;
        });
    }

    public UpdateDistributorFactory WithInterceptor(string[] commandsToIntercept)
    {
        return WithCustomPostUpdateListenerBuildAction<UpdateListener>((args, listener) =>
        {
            UpdateInterceptor interceptor = new(listener, commandsToIntercept);

            return interceptor;
        });
    }


    public UpdateDistributorFactory WithCustomPostUpdateListenerBuildAction<THandler>(Func<UpdateDistributorNextHandlerBuildArgs, THandler, IHandler<Update>> action)
        where THandler : IHandler<Update>
    {
        return WithCustomPostUpdateListenerBuildAction((args, handler) =>
        {
            if (handler is THandler typeSafeHandler)
            {
                return action.Invoke(args, typeSafeHandler);
            }

            throw new InvalidCastException($"the {nameof(handler)} has a wrong type.");
        });
    } 

    public UpdateDistributorFactory WithCustomPostUpdateListenerBuildAction(Func<UpdateDistributorNextHandlerBuildArgs, IHandler<Update>, IHandler<Update>> action)
    {
        var newAction = (UpdateDistributorNextHandlerBuildArgs args, IHandler<Update> handler) =>
        {
            var previousPostBuildAction = _postBuildUpdateListenerSetup;
            var previousPostBuildActionResult = previousPostBuildAction.Invoke(args, handler);

            return action.Invoke(args, previousPostBuildActionResult);
        };

        _postBuildUpdateListenerSetup = newAction;

        return this;
    }


    public UpdateDistributorFactory(Func<IChatIdProvider, IMessageService<Message>> messageServiceProvider,
                                    Func<IChatIdProvider, IMessageService<string>> messageServiceStringProvider,
                                    Func<IChatIdProvider, IMessageService<Tuple<string, KeyboardButton>>> messageServiceWithReplyKeyboardProvider,
                                    Func<IChatIdProvider, IReplyMarkupManager> replyMarkupManagerProvider,
                                    Func<IChatIdProvider, IAuthenticationService> authenticationServiceProvider)
    {
        _messageServiceProvider = messageServiceProvider;
        _messageServiceStringProvider = messageServiceStringProvider;
        _messageServiceWithReplyKeyboardProvider = messageServiceWithReplyKeyboardProvider;

        _replyMarkupManagerProvider = replyMarkupManagerProvider;
        _authenticationServiceProvider = authenticationServiceProvider;

        _postBuildUpdateListenerSetup = (args, listener) =>
        {
            return listener;
        };
    }
}