using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInput;

public sealed class FileInputCommandStepOptions
{
    public required StepBaseOptions<FileInputStepViewModel> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, FileReference, Task> OnUserCommit { get; init; }


    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";
    public string NoFileProvidedMessage { get; init; } = "This command accepts only file messages";
    public string FileExceedsMaxFileSizeMessage { get; init; } = "Bots can only work with files less than 20MB";
    public string FileNotAvailableMessage { get; init; } = "File is not available. please try again";

    /// <summary>
    /// If you are not hosting your telegram bot api locally, the max file size is 20MB, this boolean allows you to override this check.
    /// </summary>
    public bool EnforceMaxFileSize { get; init; } = true;
}