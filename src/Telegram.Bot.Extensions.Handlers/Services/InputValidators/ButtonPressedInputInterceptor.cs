using LisBot.Common.Telegram.ViewModels.CallbackQuery;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public abstract class ButtonPressedInputInterceptor : IUserInputValidator<CallbackQueryViewModel>
{
    private readonly CallbackQueryViewModel _buttonToPress;

    public async Task<bool> ValidateUserInput(CallbackQueryViewModel update)
    {
        if (update.CID == _buttonToPress.CID && update.BID == _buttonToPress.BID)
        {
            await OnButtonPressed();
            return false;
        }

        return true;
    }

    protected abstract Task OnButtonPressed();

    public ButtonPressedInputInterceptor(CallbackQueryViewModel buttonToPress)
    {
        _buttonToPress = buttonToPress;
    }
}
