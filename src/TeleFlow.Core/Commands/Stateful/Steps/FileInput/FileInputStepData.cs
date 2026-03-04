using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Files;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInput;

public record FileInputStepData(FileReference? FileSent) : StepData
{
    public static FileInputStepData Default
        => new(FileSent: null);
}