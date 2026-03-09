using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed record ListSelectionStepOptions<T>
{
    public required CallbackStepOptions<ListSelectionStepData<T>> BaseOptions { get; init; }

    public required Func<IServiceProvider, Task<IEnumerable<T>>> ValueProvider { get; init; }

    public required ListSelectionMode Mode { get; init; }

    public string? IndexOutOfRangeMessage { get; init; }
    public string? LastPageMessage { get; init; } 
    public string? FirstPageMessage { get; init; } 

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