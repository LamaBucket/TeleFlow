using Telegram.Bot.Extensions.Handlers.Models;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace demo.Services;

public class UpdateDistributorArgsBuilder : IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs>
{
    private readonly IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message> _messageServiceFactory;

    private readonly IMessageServiceFactory<string> _messageServiceStringFactory;

    private readonly IMessageServiceFactory<ImageMessageServiceMessage> _messageServiceImagesFactory;


    private readonly IReplyMarkupManagerFactory _replyMarkupManagerFactory;

    private readonly InlineMarkupManagerFactory _inlineMarkupManagerFactory;


    private readonly IMediaDownloaderServiceFactory _mediaDownloaderServiceFactory;


    public UpdateDistributorNextHandlerBuildArgs Build(IChatIdProvider chatIdProvider)
    {
        var chatId = chatIdProvider.GetChatId();
        return new(_messageServiceStringFactory.CreateMessageService(chatId),
                   _messageServiceImagesFactory.CreateMessageService(chatId),
                   _messageServiceFactory.CreateMessageService(chatId),
                   _replyMarkupManagerFactory.CreateReplyMarkupManager(chatId),
                   _inlineMarkupManagerFactory.Create(chatId),
                   _mediaDownloaderServiceFactory.CreateMediaDownloaderService(),
                   chatIdProvider);
    }

    public UpdateDistributorArgsBuilder(IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message> messageServiceFactory,
                                        IMessageServiceFactory<string> messageServiceStringFactory,
                                        IMessageServiceFactory<ImageMessageServiceMessage> messageServiceImagesFactory,
                                        IReplyMarkupManagerFactory replyMarkupManagerFactory,
                                        InlineMarkupManagerFactory inlineMarkupManagerFactory,
                                        IMediaDownloaderServiceFactory mediaDownloaderServiceFactory)
    {
        _messageServiceFactory = messageServiceFactory;
        _messageServiceStringFactory = messageServiceStringFactory;
        _messageServiceImagesFactory = messageServiceImagesFactory;
        _replyMarkupManagerFactory = replyMarkupManagerFactory;
        _inlineMarkupManagerFactory = inlineMarkupManagerFactory;
        _mediaDownloaderServiceFactory = mediaDownloaderServiceFactory;
    }
}