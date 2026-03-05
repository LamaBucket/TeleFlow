using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed class ListSelectionStepOptions<T>
{
    public required CallbackStepOptions<ListSelectionStepData<T>> BaseOptions { get; init; }

    public required Func<IServiceProvider, Task<IReadOnlyList<T>>> ValueProvider { get; init; }

    public required ListSelectionMode Mode { get; init; }

    public string? IndexOutOfRangeMessage { get; init; } = "Please select an item from the list";
    public string? LastPageMessage { get; init; } = "There is no more items";
    public string? FirstPageMessage { get; init; } = "There is no more items";

}

public abstract class ListSelectionMode
{
    public sealed class SingleSelect<T> : ListSelectionMode
    {
        public required Func<CommandStepCommitContext, T, Task> OnCommit { get; init; }
    }

    public sealed class MultiSelect<T> : ListSelectionMode
    {
        public required Func<CommandStepCommitContext, IEnumerable<T>, Task> OnCommit { get; init; }
    }
}