using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Models;

public class UpdateDistributorNextHandlerBuildArgs
{
    public IMessageService<string> MessageServiceString { get; init; }

    public IMessageService<Message> MessageService { get; init; }

    public IMessageService<Tuple<string, KeyboardButton>> MessageServiceWithReplyButton { get; init; }

    public IReplyMarkupManager ReplyMarkupManager { get; init; }


    public IAuthenticationService AuthenticationService { get; init; }

    public IChatIdProvider ChatIdProvider { get; init; }


    public UpdateDistributorNextHandlerBuildArgs(IMessageService<string> messageServiceString,
                                                 IMessageService<Message> messageService,
                                                 IMessageService<Tuple<string, KeyboardButton>> messageServiceWithReplyButton,
                                                 IReplyMarkupManager replyMarkupManager,
                                                 IAuthenticationService authenticationService,
                                                 IChatIdProvider chatIdProvider)
    {
        MessageServiceString = messageServiceString;
        MessageService = messageService;
        MessageServiceWithReplyButton = messageServiceWithReplyButton;

        ReplyMarkupManager = replyMarkupManager;

        AuthenticationService = authenticationService;
        ChatIdProvider = chatIdProvider;
    }
}