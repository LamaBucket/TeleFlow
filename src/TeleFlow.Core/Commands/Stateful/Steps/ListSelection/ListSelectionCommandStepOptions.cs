using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase.Internal;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed class ListSelectionCommandStepOptions<T>
{
    public required Func<IServiceProvider, Task<IReadOnlyList<T>>> ValueProvider { get; init; }
    public required Func<T, string> DisplayNameParser { get; init; }
    public required ListSelectionMode Mode { get; init; }

    public int MaxItemsInRow { get; init; } = 2;
    public int MaxRowsInPage { get; init; } = 5;
    public bool FitItemsOnLastPage { get; set; } = false;


    public string PrevPageButtonText { get; init; } = DefaultButtonTexts.PrevPageButtonText;

    public string NextPageButtonText { get; init; } = DefaultButtonTexts.NextPageButtonText;

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
        public required Func<CommandStepCommitContext, IReadOnlyList<T>, Task> OnCommit { get; init; }

        public string FinishButtonText { get; init; } = DefaultButtonTexts.DoneButtonText;
    }
}