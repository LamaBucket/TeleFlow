using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class ExecutePreviousCommandInterceptor : CallbackButtonPressedInputInterceptor
{
    private readonly StepChainBuilder _stepChainBuilder;
    
    
    protected override async Task InterceptAction()
    {
        var last = _stepChainBuilder.RebuildWithPreviousStep();

        await last.OnCommandCreated();
    }

    public ExecutePreviousCommandInterceptor(CallbackQueryViewModel buttonToPress,
                                             StepChainBuilder stepChainBuilder) : base(buttonToPress)
    {
        _stepChainBuilder = stepChainBuilder;
    }
}
