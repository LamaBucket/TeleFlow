namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;

public sealed class ListSelectionOptions<T>
{
    public required Func<IServiceProvider, Task<IReadOnlyList<T>>> ValueProvider { get; init; }

    public required Func<T, string> DisplayNameParser { get; init; }

    public required ListSelectionMode Mode { get; init; }


    public ListSelectionPageSizeOptions PageSizeOptions { get; init; } = new();

    public ListSelectionButtonTextOptions ButtonTextOptions { get; init; } = new();

    public ListSelectionErrorMessageOptions ErrorMessageOptions { get; init; } = new();

}

public abstract class ListSelectionMode
{
    public sealed class SingleSelect<T> : ListSelectionMode
    {
        public required Func<CommandStepCommitContext, T, Task> OnCommit { get; init; }
    }

    public sealed class MultiSelect<T> : ListSelectionMode
    {
        public required Func<CommandStepCommitContext, IReadOnlyList<T>, Task> OnCommit { get; init; }
    }
}