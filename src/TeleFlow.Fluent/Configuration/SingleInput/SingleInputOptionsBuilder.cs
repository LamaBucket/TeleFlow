using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base.Render;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.SingleInput;

public class SingleInputOptionsBuilder<TInput>
{
    internal InputStepRenderOptions<TInput> StepRenderOptions => _stepRenderOptions;
    
    private InputStepRenderOptions<TInput> _stepRenderOptions;
    
    
    

    public void WithParseMode(ParseMode parseMode)
    {
        _stepRenderOptions = _stepRenderOptions with { ParseMode = parseMode };
    }
    
    public void WithUserPrompt(InputStepUserPrompt<TInput?> userPrompt)
    {
        _stepRenderOptions = _stepRenderOptions with { UserPrompt = userPrompt };
    }
    
    private SingleInputOptionsBuilder(InputStepRenderOptions<TInput> stepRenderOptions)
    {
        _stepRenderOptions = stepRenderOptions;
    }
}