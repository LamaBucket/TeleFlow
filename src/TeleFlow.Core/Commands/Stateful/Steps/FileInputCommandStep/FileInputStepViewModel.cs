using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Files;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInputCommandStep;

public record FileInputStepViewModel(FileReference? FileSent) : StepViewModel
{
    public static FileInputStepViewModel Default
        => new(FileSent: null);
}