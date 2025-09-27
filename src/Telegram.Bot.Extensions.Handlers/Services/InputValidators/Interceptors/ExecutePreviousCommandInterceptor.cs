using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class ExecutePreviousCommandInterceptor : IUserInputValidator<CallbackQueryViewModel>
{
    private readonly StepChainBuilder _stepChainBuilder;
    
    private readonly CallbackQueryViewModel _buttonToPress;


    public async Task<bool> ValidateUserInput(CallbackQueryViewModel update)
    {
        if (update.CID == _buttonToPress.CID && update.BID == _buttonToPress.BID)
        {
            await RestartPreviousStepInStepManager();

            return false;
        }

        return true;
    }

    protected async Task RestartPreviousStepInStepManager()
    {
        var last = _stepChainBuilder.RebuildWithPreviousStep();

        await last.OnCommandCreated();
    }


    public ExecutePreviousCommandInterceptor(CallbackQueryViewModel buttonToPress,
                                             StepChainBuilder stepChainBuilder)
    {
        _buttonToPress = buttonToPress;
        _stepChainBuilder = stepChainBuilder;
    }
}
