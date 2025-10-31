using TeleFlow.Services;
using TeleFlow.Services.Markup;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Models;

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