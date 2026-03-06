using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;
using TeleFlow.Extensions.DI.Toolkit.Commands.Steps.BaseConfigs;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.FileInput;

public class FileInputToolkitConfig : StatefulStepToolkitConfig
{
    public FileInputToolkitRenderConfig Render { get; init; } = new();

    public bool EnforceMaxFileSize { get; set; } = FileInputDefaults.EnforceMaxFileSize;
    public string? NoMessageInputMessage { get; set; } = FileInputDefaults.NoMessageInputMessage;
    public string? NoFileProvidedMessage { get; set; } = FileInputDefaults.NoFileProvidedMessage;
    public string? FileExceedsMaxFileSizeMessage { get; set; } = FileInputDefaults.FileExceedsMaxFileSizeMessage;
    public string? FileNotAvailableMessage { get; set; }   = FileInputDefaults.FileNotAvailableMessage;  


    public FileInputStepOptions BuildOptions(IStepRenderService<FileInputStepData> renderService, Func<CommandStepCommitContext, FileReference, Task> onCommit)  
        => new()
        {
            RenderConfig = BuildStatefulOptions(renderService),
            OnUserCommit = onCommit,
            EnforceMaxFileSize = EnforceMaxFileSize,
            NoMessageInputMessage = NoMessageInputMessage,
            NoFileProvidedMessage = NoFileProvidedMessage,
            FileExceedsMaxFileSizeMessage = FileExceedsMaxFileSizeMessage,
            FileNotAvailableMessage = FileNotAvailableMessage
        };

    public FileInputRenderServiceOptions BuildRenderOptions()
        => new()
        {
            ParseMode      = Render.ParseMode,
            PromptText     = Render.PromptText,
            AfterInputText = Render.AfterInputText
        };
      
}

public class FileInputToolkitRenderConfig
{
    public ParseMode ParseMode { get; set; } = FileInputDefaults.ParseMode;

    public Func<IServiceProvider, string> PromptText { get; set; } = FileInputDefaults.PromptText;
    public Func<IServiceProvider, FileReference, string>? AfterInputText { get; set; } = FileInputDefaults.AfterInputText;
}