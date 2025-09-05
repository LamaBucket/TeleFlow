using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Models;

public class UpdateListenerCommandExecutionArgs
{
    public NavigatorFactoryArgs NavigatorArgs { get; init; }


    public IMessageService<string> MessageServiceString => _buildTimeArgs.FromUpdateDistributorArgs.MessageServiceString;

    public IMessageServiceWithEdit<Message> MessageService => _buildTimeArgs.FromUpdateDistributorArgs.MessageService;

    public IReplyMarkupManager ReplyMarkupManager => _buildTimeArgs.FromUpdateDistributorArgs.ReplyMarkupManager;


    public IChatIdProvider ChatIdProvider => _buildTimeArgs.FromUpdateDistributorArgs.ChatIdProvider;

    public IAuthenticationService AuthenticationService => _buildTimeArgs.FromUpdateDistributorArgs.AuthenticationService;


    public INavigatorHandler Navigator => _buildTimeArgs.Navigator;


    private readonly UpdateListenerCommandBuildArgs _buildTimeArgs;


    public UpdateListenerCommandExecutionArgs(NavigatorFactoryArgs navigatorArgs,
                                              UpdateListenerCommandBuildArgs buildTimeArgs)
    {
        NavigatorArgs = navigatorArgs;
        _buildTimeArgs = buildTimeArgs;
    }
}