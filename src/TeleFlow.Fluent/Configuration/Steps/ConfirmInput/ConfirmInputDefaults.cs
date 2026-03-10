using System.Text;
using TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput.Render;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.Steps.ConfirmInput;

public static class ConfirmInputDefaults
{
    public static readonly string InvalidButtonMessage = "Invalid button message";
    
    public static readonly ParseMode ParseMode = ParseMode.None; 
    
    public static readonly Func<IServiceProvider, bool?, string> UserPrompt = DefaultUserPrompt;

    public static readonly Func<bool, string> ConfirmButtonsParser = (val) => val ? "Ok" : "Cancel";

    public static readonly ConfirmInputButtonsDirection Direction = ConfirmInputButtonsDirection.Horizontal;

    
    private static string DefaultUserPrompt(IServiceProvider sp, bool? value)
    {
        StringBuilder sb = new();

        sb.AppendLine("Please confirm your action:");

        if (value.HasValue)
        {
            sb.AppendLine(value.Value ? "You confirmed" : "You cancelled");
        }
        
        return sb.ToString();
    }
}