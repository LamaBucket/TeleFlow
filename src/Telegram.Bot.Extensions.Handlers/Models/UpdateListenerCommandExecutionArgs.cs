using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Models;

public class UpdateListenerCommandExecutionArgs
{
    public NavigatorFactoryArgs NavigatorArgs { get; init; }


    public IMessageService<string> MessageServiceString => BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString;

    public IMessageServiceWithEdit<Message> MessageService => BuildTimeArgs.FromUpdateDistributorArgs.MessageService;

    public IReplyMarkupManager ReplyMarkupManager => BuildTimeArgs.FromUpdateDistributorArgs.ReplyMarkupManager;


    public IChatIdProvider ChatIdProvider => BuildTimeArgs.FromUpdateDistributorArgs.ChatIdProvider;

    public IAuthenticationService AuthenticationService => BuildTimeArgs.FromUpdateDistributorArgs.AuthenticationService;


    public INavigatorHandler Navigator => BuildTimeArgs.Navigator;


    public UpdateListenerCommandBuildArgs BuildTimeArgs { get; init; }


    public UpdateListenerCommandExecutionArgs(NavigatorFactoryArgs navigatorArgs,
                                              UpdateListenerCommandBuildArgs buildTimeArgs)
    {
        NavigatorArgs = navigatorArgs;
        BuildTimeArgs = buildTimeArgs;
    }
}