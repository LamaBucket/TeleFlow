using System.ComponentModel;
using System.Diagnostics;
using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Factories;

public class UpdateDistributorFactory : IHandlerFactory<UpdateDistributor, Update>
{
    protected UpdateDistributor? Instance { get; set; }


    private readonly Action<UpdateListenerCommandFactoryBuilder> _setupBuilder;


    private readonly Func<IChatIdProvider, IMessageService<Message>> _messageServiceProvider;

    private readonly Func<IChatIdProvider, IMessageService<string>> _messageServiceStringProvider;

    private readonly Func<IChatIdProvider, IMessageService<Tuple<string, KeyboardButton>>> _messageServiceWithReplyKeyboardProvider;


    private readonly Func<IChatIdProvider, IReplyMarkupManager> _replyMarkupManagerProvider;

    private readonly Func<IChatIdProvider, IAuthenticationService> _authenticationServiceProvider;


    private Func<IHandler<Update>, IHandler<Update>> _postBuildUpdateListenerSetup;


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
        var listenerFactory = new UpdateListenerFactory((chatIdProvider, navHandler) =>
        {
            var updateListenerFactoryBuilder = new UpdateListenerCommandFactoryBuilder();

            _setupBuilder.Invoke(updateListenerFactoryBuilder);

            UpdateListenerFactoryBuildArgs args = new(_messageServiceStringProvider.Invoke(chatIdProvider),
                                                      _messageServiceProvider.Invoke(chatIdProvider),
                                                      _messageServiceWithReplyKeyboardProvider.Invoke(chatIdProvider),
                                                      chatIdProvider,
                                                      navHandler,
                                                      _replyMarkupManagerProvider.Invoke(chatIdProvider),
                                                      _authenticationServiceProvider.Invoke(chatIdProvider));

            return updateListenerFactoryBuilder.Build(args);

        });

        return new UpdateDistributor(listenerFactory, _postBuildUpdateListenerSetup);
    }

    protected virtual void BuildFinished(UpdateDistributor buildResult)
    {
        Instance = buildResult;

        // if you need the update distributor not to be Singleton - clear the created instance here.
    }


    public UpdateDistributorFactory WithExceptionHandler(Func<Exception, Task> handlerAction)
    {
        return WithCustomPostUpdateListenerBuildAction((handler) =>
        {
            UpdateExceptionHandler exceptionHandler = new(handler, handlerAction);

            return exceptionHandler;
        });
    }

    public UpdateDistributorFactory WithInterceptor(string[] commandsToIntercept)
    {
        return WithCustomPostUpdateListenerBuildAction<UpdateListener>((listener) =>
        {
            UpdateInterceptor interceptor = new(listener, commandsToIntercept);

            return interceptor;
        });
    }


    public UpdateDistributorFactory WithCustomPostUpdateListenerBuildAction<THandler>(Func<THandler, IHandler<Update>> action)
        where THandler : IHandler<Update>
    {
        return WithCustomPostUpdateListenerBuildAction((handler) =>
        {
            if (handler is THandler typeSafeHandler)
            {
                return action.Invoke(typeSafeHandler);
            }

            throw new InvalidCastException($"the {nameof(handler)} has a wrong type.");
        });
    } 

    public UpdateDistributorFactory WithCustomPostUpdateListenerBuildAction(Func<IHandler<Update>, IHandler<Update>> action)
    {
        var newAction = (IHandler<Update> handler) =>
        {
            var currentPostBuildAction = _postBuildUpdateListenerSetup;


            return action.Invoke(currentPostBuildAction.Invoke(handler));
        };

        _postBuildUpdateListenerSetup = newAction;

        return this;
    }


    public UpdateDistributorFactory(Func<IChatIdProvider, IMessageService<Message>> messageServiceProvider,
                                    Func<IChatIdProvider, IMessageService<string>> messageServiceStringProvider,
                                    Func<IChatIdProvider, IMessageService<Tuple<string, KeyboardButton>>> messageServiceWithReplyKeyboardProvider,
                                    Func<IChatIdProvider, IReplyMarkupManager> replyMarkupManagerProvider,
                                    Func<IChatIdProvider, IAuthenticationService> authenticationServiceProvider,
                                    Action<UpdateListenerCommandFactoryBuilder> setupBuilder)
    {
        _messageServiceProvider = messageServiceProvider;
        _messageServiceStringProvider = messageServiceStringProvider;
        _messageServiceWithReplyKeyboardProvider = messageServiceWithReplyKeyboardProvider;

        _replyMarkupManagerProvider = replyMarkupManagerProvider;
        _authenticationServiceProvider = authenticationServiceProvider;

        _setupBuilder = setupBuilder;

        _postBuildUpdateListenerSetup = (listener) =>
        {
            return listener;
        };
    }
}