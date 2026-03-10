using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput.Render;

public record ConfirmInputRenderServiceOptions
{
    public required ParseMode ParseMode { get; init; }
    
    public required Func<IServiceProvider, bool?, string> UserPrompt { get; init; }
    
    public required Func<bool, string> ConfirmButtonsParser { get; init; }

    public ConfirmInputButtonsDirection Direction { get; init; }
}

public enum ConfirmInputButtonsDirection
{
    Horizontal,
    Vertical
}