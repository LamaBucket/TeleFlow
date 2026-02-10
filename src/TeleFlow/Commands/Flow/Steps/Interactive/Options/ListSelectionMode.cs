using TeleFlow.Models.MultiStep;

namespace TeleFlow.Commands.Flow.Steps.Interactive.Options;

public abstract class ListSelectionMode<T>
{
    public sealed class Single : ListSelectionMode<T>
    {
        public required Func<FlowStepCommitContext, T, Task> OnCommit { get; init; }
    }

    public sealed class Multi : ListSelectionMode<T>
    {
        public required Func<FlowStepCommitContext, IReadOnlyList<T>, Task> OnCommit { get; init; }
    }
}