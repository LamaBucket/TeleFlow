using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Models;

public class UpdateDistributorNextHandlerBuildArgs
{
    public IMessageService<string> MessageServiceString { get; init; }

    public IMessageServiceWithEdit<Message> MessageService { get; init; }

    public IReplyMarkupManager ReplyMarkupManager { get; init; }


    public IAuthenticationService AuthenticationService { get; init; }

    public IChatIdProvider ChatIdProvider { get; init; }


    public UpdateDistributorNextHandlerBuildArgs(IMessageService<string> messageServiceString,
                                                 IMessageServiceWithEdit<Message> messageService,
                                                 IReplyMarkupManager replyMarkupManager,
                                                 IAuthenticationService authenticationService,
                                                 IChatIdProvider chatIdProvider)
    {
        MessageServiceString = messageServiceString;
        MessageService = messageService;

        ReplyMarkupManager = replyMarkupManager;

        AuthenticationService = authenticationService;
        ChatIdProvider = chatIdProvider;
    }
}