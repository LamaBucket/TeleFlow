using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class GoToPreviousButtonInputInterceptor : ButtonPressedInputInterceptor
{
    private readonly StepChainBuilder _stepChainBuilder;

    protected override async Task OnButtonPressed()
    {
        var last = _stepChainBuilder.RebuildWithPreviousStep();

        await last.OnCommandCreated();
    }

    
    public GoToPreviousButtonInputInterceptor(CallbackQueryViewModel buttonToPress, StepChainBuilder stepChainBuilder) : base(buttonToPress)
    {
        _stepChainBuilder = stepChainBuilder;
    }
}
