namespace TeleFlow.Commands.Flow.Steps.Interactive.Options;

public sealed class ListSelectionStepOptions<T>
{
    public required Func<IServiceProvider, Task<IReadOnlyList<T>>> ValueProvider { get; init; }
    public required Func<T, string> DisplayNameParser { get; init; }
    public required ListSelectionMode<T> Mode { get; init; }

    public int MaxItemsInRow { get; init; } = 2;
    public int MaxRowsInPage { get; init; } = 5;
}