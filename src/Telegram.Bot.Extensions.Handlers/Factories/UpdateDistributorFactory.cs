using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Factories;

public class UpdateDistributorFactory : IHandlerFactory<UpdateDistributor, Update>
{
    protected UpdateDistributor? Instance { get; set; }


    private readonly IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message> _messageServiceFactory;

    private readonly IMessageServiceFactory<string> _messageServiceStringFactory;

    private readonly IMessageServiceFactory<ImageMessageServiceMessage> _messageServiceImageFactory;


    private readonly IReplyMarkupManagerFactory _replyMarkupManagerFactory;

    private readonly InlineMarkupManagerFactory _inlineMarkupManagerFactory;


    private readonly IAuthenticationServiceFactory _authenticationServiceFactory;


    private readonly IMediaDownloaderServiceFactory _mediaDownloaderServiceFactory;


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
                                                             _messageServiceImageFactory.CreateMessageService(chatIdProvider.GetChatId()),
                                                             _messageServiceFactory.CreateMessageService(chatIdProvider.GetChatId()),
                                                             _replyMarkupManagerFactory.CreateReplyMarkupManager(chatIdProvider.GetChatId()),
                                                             _inlineMarkupManagerFactory.Create(chatIdProvider.GetChatId()),
                                                             _mediaDownloaderServiceFactory.CreateMediaDownloaderService(),
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

    public UpdateDistributorFactory(IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message> messageServiceFactory,
                                    IMessageServiceFactory<string> messageServiceStringFactory,
                                    IMessageServiceFactory<ImageMessageServiceMessage> messageServiceImageFactory,
                                    IReplyMarkupManagerFactory replyMarkupManagerFactory,
                                    InlineMarkupManagerFactory inlineMarkupManagerFactory,
                                    IMediaDownloaderServiceFactory mediaDownloaderServiceFactory,
                                    IAuthenticationServiceFactory authenticationServiceFactory)
    {
        _messageServiceFactory = messageServiceFactory;
        _messageServiceStringFactory = messageServiceStringFactory;
        _messageServiceImageFactory = messageServiceImageFactory;

        _replyMarkupManagerFactory = replyMarkupManagerFactory;
        _inlineMarkupManagerFactory = inlineMarkupManagerFactory;

        _mediaDownloaderServiceFactory = mediaDownloaderServiceFactory;

        _authenticationServiceFactory = authenticationServiceFactory;
    }
}