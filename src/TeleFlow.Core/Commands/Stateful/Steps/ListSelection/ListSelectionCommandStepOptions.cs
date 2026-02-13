namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed class ListSelectionCommandStepOptions<T>
{
    public required Func<IServiceProvider, Task<IReadOnlyList<T>>> ValueProvider { get; init; }
    public required Func<T, string> DisplayNameParser { get; init; }
    public required ListSelectionMode<T> Mode { get; init; }

    public int MaxItemsInRow { get; init; } = 2;
    public int MaxRowsInPage { get; init; } = 5;
}

public abstract class ListSelectionMode<T>
{
    public sealed class Single : ListSelectionMode<T>
    {
        public required Func<CommandStepCommitContext, T, Task> OnCommit { get; init; }
    }

    public sealed class Multi : ListSelectionMode<T>
    {
        public required Func<CommandStepCommitContext, IReadOnlyList<T>, Task> OnCommit { get; init; }
    }
}