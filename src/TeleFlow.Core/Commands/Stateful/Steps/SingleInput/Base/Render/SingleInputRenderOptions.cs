using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base.Render;

public delegate string SingleInputStepUserPrompt<in TInput>(IServiceProvider sp, TInput? input);

public record SingleInputRenderOptions<TInput> 
{
    public required ParseMode ParseMode { get; init; }
    
    public required SingleInputStepUserPrompt<TInput?> UserPrompt { get; init; }
}