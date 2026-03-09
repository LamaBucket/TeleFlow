using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.FileInput;

public sealed class FileInputStepOptions
{
    public required StatefulStepOptions<SingleInputStepData<FileReference>> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, FileReference, Task> OnUserCommit { get; init; }

    /// <summary>
    /// If you are not hosting your telegram bot api locally, the max file size is 20MB, this boolean allows you to override this check.
    /// </summary>
    public required bool EnforceMaxFileSize { get; init; }

    public string? NoMessageInputMessage { get; init; }
    public string? NoFileProvidedMessage { get; init; }
    public string? FileExceedsMaxFileSizeMessage { get; init; }
    public string? FileNotAvailableMessage { get; init; }
    
}