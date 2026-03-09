using TeleFlow.Abstractions.Transport.Files;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.Steps.FileInput;

public class FileInputToolkitRenderConfig
{
    public ParseMode ParseMode { get; set; } = FileInputDefaults.ParseMode;

    public Func<IServiceProvider, string> PromptText { get; set; } = FileInputDefaults.PromptText;
    public Func<IServiceProvider, FileReference, string>? AfterInputText { get; set; } = FileInputDefaults.AfterInputText;
}