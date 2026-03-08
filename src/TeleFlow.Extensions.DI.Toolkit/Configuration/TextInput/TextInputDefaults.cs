using Telegram.Bot.Types.Enums;

namespace TeleFlow.Extensions.DI.Toolkit.Configuration.TextInput;

internal static class TextInputDefaults
{
    public static readonly string NoMessageInputMessage = "This Command accepts only messages";
    public static readonly string NoTextProvidedMessage = "This command accepts only text";


    public static readonly ParseMode ParseMode = ParseMode.None;
    public static readonly Func<IServiceProvider, string> PromptText = _ => "Please enter some text";
    public static readonly Func<IServiceProvider, string, string> AfterInputText = (_, input) => $"You entered: {input}";
}