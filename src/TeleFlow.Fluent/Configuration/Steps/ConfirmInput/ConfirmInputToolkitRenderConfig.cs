using TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput.Render;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.Steps.ConfirmInput;

public class ConfirmInputToolkitRenderConfig
{
    public ParseMode ParseMode { get; set; }  = ConfirmInputDefaults.ParseMode;
    
    public Func<IServiceProvider, bool?, string> UserPrompt { get; set; } = ConfirmInputDefaults.UserPrompt;
    
    public Func<bool, string> ConfirmButtonsParser { get; set; } = ConfirmInputDefaults.ConfirmButtonsParser;

    public ConfirmInputButtonsDirection Direction { get; set; } = ConfirmInputDefaults.Direction;
}