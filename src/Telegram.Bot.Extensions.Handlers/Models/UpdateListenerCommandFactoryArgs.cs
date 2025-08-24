using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Models;

public class UpdateListenerCommandFactoryArgs
{
    public NavigatorFactoryArgs NavigatorArgs { get; init; }

    public IMessageService<string> MessageServiceString { get; init; }

    public IMessageService<Message> MessageService { get; init; }

    public IMessageService<Tuple<string, KeyboardButton>> MessageServiceWithReplyButton { get; init; }

    public IReplyMarkupManager ReplyMarkupManager { get; init; }

    public INavigatorHandler Navigator { get; init; }

    public IChatIdProvider ChatIdProvider { get; init; }

    public IAuthenticationService AuthenticationService { get; init; }

    public UpdateListenerCommandFactoryArgs(NavigatorFactoryArgs navigatorArgs,
                                            IMessageService<string> messageServiceString,
                                            IMessageService<Message> messageService,
                                            IMessageService<Tuple<string, KeyboardButton>> messageServiceWithReplyButton,
                                            IChatIdProvider chatIdProvider,
                                            INavigatorHandler navigator,
                                            IReplyMarkupManager replyMarkupManager,
                                            IAuthenticationService authenticationService)
    {
        NavigatorArgs = navigatorArgs;
        MessageServiceString = messageServiceString;
        MessageService = messageService;
        MessageServiceWithReplyButton = messageServiceWithReplyButton;
        ChatIdProvider = chatIdProvider;
        Navigator = navigator;
        ReplyMarkupManager = replyMarkupManager;
        AuthenticationService = authenticationService;
    }
}