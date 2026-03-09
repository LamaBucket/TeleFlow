using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput.Render;

public sealed record TextInputRenderServiceOptions
{
    public required ParseMode ParseMode { get; init; }
    
    public required Func<IServiceProvider, string> PromptText { get; init; }
    
    public Func<IServiceProvider, string, string>? AfterInputText { get; init; }

}