using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Extensions.DI.Toolkit.Configuration.FileInput;

public class FileInputToolkitRenderConfig
{
    public ParseMode ParseMode { get; set; } = FileInputDefaults.ParseMode;

    public Func<IServiceProvider, string> PromptText { get; set; } = FileInputDefaults.PromptText;
    public Func<IServiceProvider, FileReference, string>? AfterInputText { get; set; } = FileInputDefaults.AfterInputText;
}