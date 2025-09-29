using LisBot.Common.Telegram;
using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class NavigateCommandInterceptor : CallbackButtonPressedInputInterceptor
{    
    private readonly INavigatorHandler _navigator;

    private readonly string _pathToNavigate;


    protected override async Task InterceptAction()
    {
        await _navigator.Handle(_pathToNavigate);
    }


    public NavigateCommandInterceptor(CallbackQueryViewModel buttonToPress, INavigatorHandler navigator, string pathToNavigate) : base(buttonToPress)
    {
        _navigator = navigator;
        _pathToNavigate = pathToNavigate;

    }
}
