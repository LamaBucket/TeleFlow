using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;
using TeleFlow.Fluent.Configuration.Steps.Base;

namespace TeleFlow.Fluent.Configuration.Steps.FileInput;

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