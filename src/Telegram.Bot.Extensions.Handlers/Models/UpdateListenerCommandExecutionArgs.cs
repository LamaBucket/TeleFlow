using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Models;

public class UpdateListenerCommandExecutionArgs<TBuildArgs> where TBuildArgs : class
{
    public NavigatorFactoryArgs NavigatorArgs { get; init; }

    public UpdateListenerCommandBuildArgs<TBuildArgs> BuildTimeArgs { get; init; }


    public UpdateListenerCommandExecutionArgs(NavigatorFactoryArgs navigatorArgs,
                                              UpdateListenerCommandBuildArgs<TBuildArgs> buildTimeArgs)
    {
        NavigatorArgs = navigatorArgs;
        BuildTimeArgs = buildTimeArgs;
    }
}