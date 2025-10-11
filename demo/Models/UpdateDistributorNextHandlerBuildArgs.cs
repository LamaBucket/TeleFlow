using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Extensions.Handlers.Models;

public class UpdateDistributorNextHandlerBuildArgs
{
    public IMessageService<string> MessageServiceString { get; init; }

    public IMessageService<ImageMessageServiceMessage> MessageServiceImages { get; init; }

    public IMessageServiceWithEdit<Message> MessageService { get; init; }


    public IReplyMarkupManager ReplyMarkupManager { get; init; }

    public InlineMarkupManager InlineMarkupManager { get; init; }


    public IMediaDownloaderService MediaDownloaderService { get; init; }


    public IChatIdProvider ChatIdProvider { get; init; }


    public UpdateDistributorNextHandlerBuildArgs(IMessageService<string> messageServiceString,
                                                 IMessageService<ImageMessageServiceMessage> messageServiceImages,
                                                 IMessageServiceWithEdit<Message> messageService,
                                                 IReplyMarkupManager replyMarkupManager,
                                                 InlineMarkupManager inlineMarkupManager,
                                                 IMediaDownloaderService mediaDownloaderService,
                                                 IChatIdProvider chatIdProvider)
    {
        MessageServiceString = messageServiceString;
        MessageServiceImages = messageServiceImages;
        MessageService = messageService;

        ReplyMarkupManager = replyMarkupManager;
        InlineMarkupManager = inlineMarkupManager;
        MediaDownloaderService = mediaDownloaderService;

        ChatIdProvider = chatIdProvider;
    }
}