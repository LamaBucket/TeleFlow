using TeleFlow.Abstractions.Transport.Files;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.Steps.FileInput;

internal static class FileInputDefaults
{
    public static readonly string NoMessageInputMessage = "This Command accepts only messages";
    public static readonly string NoFileProvidedMessage = "This command accepts only file messages";
    public static readonly string FileExceedsMaxFileSizeMessage = "Bots can only work with files less than 20MB";
    public static readonly string FileNotAvailableMessage = "File is not available. please try again";
    public static readonly bool EnforceMaxFileSize = true;


    public static readonly ParseMode ParseMode = ParseMode.None;
    public static readonly Func<IServiceProvider, string> PromptText = _ => "Please send us some file";
    public static readonly Func<IServiceProvider, FileReference, string> AfterInputText = (_, input) =>
    {
        if(string.IsNullOrWhiteSpace(input.FileName))
            return "You sent a file.";
        else
            return $"You sent file {input.FileName}.";
    };

}