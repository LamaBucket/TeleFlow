using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public class DateSelectionStepOptions
{
    public required CallbackStepOptions<DateSelectionStepData> BaseOptions { get; init; }

    public required DateSelectionMode Mode { get; init; }

    public string? LastPageMessage { get; init; }
    public string? FirstPageMessage { get; init; }
    
    public string? InvalidYearMessage { get; init; }
    public string? InvalidMonthMessage { get; init; }
    public string? InvalidDayMessage { get; init; }
}


public abstract class DateSelectionMode
{
    public sealed class YearOnly : DateSelectionMode
    {
        public required Func<CommandStepCommitContext, int, Task> OnCommit { get; init; }
    }

    public sealed class YearMonth : DateSelectionMode
    {
        public required Func<CommandStepCommitContext, (int year, int month), Task> OnCommit { get; init; }
    }

    public sealed class YearMonthDay : DateSelectionMode
    {
        public required Func<CommandStepCommitContext, DateOnly, Task> OnCommit { get; init; }
    }
}
