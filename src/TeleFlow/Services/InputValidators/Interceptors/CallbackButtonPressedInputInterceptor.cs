using TeleFlow.ViewModels.CallbackQuery;

namespace TeleFlow.Services.InputValidators;

public abstract class CallbackButtonPressedInputInterceptor : IUserInputValidator<CallbackQueryViewModel>
{
    private readonly CallbackQueryViewModel _buttonToPress;


    public async Task<bool> ValidateUserInput(CallbackQueryViewModel update)
    {
        if (update.CID == _buttonToPress.CID && update.BID == _buttonToPress.BID)
        {
            await InterceptAction();

            return false;
        }

        return true;
    }

    protected abstract Task InterceptAction();


    public CallbackButtonPressedInputInterceptor(CallbackQueryViewModel buttonToPress)
    {
        _buttonToPress = buttonToPress;
    }
}