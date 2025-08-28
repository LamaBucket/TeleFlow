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


    private readonly IMessageServiceFactory<Message> _messageServiceFactory;

    private readonly IMessageServiceFactory<string> _messageServiceStringFactory;


    private readonly IMessageServiceFactory<Tuple<string, KeyboardButton>> _messageServiceWithReplyKeyboardFactory;


    private readonly IReplyMarkupManagerFactory _replyMarkupManagerFactory;

    private readonly IAuthenticationServiceFactory _authenticationServiceFactory;


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
            UpdateDistributorNextHandlerBuildArgs args = new(_messageServiceStringFactory.CreateMessageService(chatIdProvider.GetChatId()),
                                                             _messageServiceFactory.CreateMessageService(chatIdProvider.GetChatId()),
                                                             _messageServiceWithReplyKeyboardFactory.CreateMessageService(chatIdProvider.GetChatId()),
                                                             _replyMarkupManagerFactory.CreateReplyMarkupManager(chatIdProvider.GetChatId()),
                                                             _authenticationServiceFactory.CreateAuthenticationService(chatIdProvider.GetChatId()),
                                                             chatIdProvider);

            UpdateDistributorNextHandlerFactoryBuilder nextHandlerBuilder = new(SetupUpdateListenerFactoryBuilder);

            ConfigureBeforeUpdateListenerHandler(nextHandlerBuilder);

            return nextHandlerBuilder.Build(args);
        });
    }

    protected virtual void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder builder)
    {

    }

    protected virtual void ConfigureBeforeUpdateListenerHandler(UpdateDistributorNextHandlerFactoryBuilder options)
    {
        
    }

    protected virtual void BuildFinished(UpdateDistributor buildResult)
    {
        Instance = buildResult;

        // if you need the update distributor not to be Singleton - clear the created instance here.
    }

    public UpdateDistributorFactory(IMessageServiceFactory<Message> messageServiceFactory,
                                    IMessageServiceFactory<string> messageServiceStringFactory,
                                    IMessageServiceFactory<Tuple<string, KeyboardButton>> messageServiceWithReplyKeyboardFactory,
                                    IReplyMarkupManagerFactory replyMarkupManagerFactory,
                                    IAuthenticationServiceFactory authenticationServiceFactory)
    {
        _messageServiceFactory = messageServiceFactory;
        _messageServiceStringFactory = messageServiceStringFactory;
        _messageServiceWithReplyKeyboardFactory = messageServiceWithReplyKeyboardFactory;
        _replyMarkupManagerFactory = replyMarkupManagerFactory;
        _authenticationServiceFactory = authenticationServiceFactory;

        _postBuildUpdateListenerSetup = (args, listener) =>
        {
            return listener;
        };
    }
}