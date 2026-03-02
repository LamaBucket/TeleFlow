using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelectionCommandStep;

public class DateSelectionStepOptions
{
    public required CallbackStepBaseOptions<DateSelectionStepViewModel> BaseOptions { get; init; }

    public required DateSelectionMode Mode { get; init; }

    public string? LastPageMessage { get; init; } = "There is no more items";
    public string? FirstPageMessage { get; init; } = "There is no more items";

    public string InvalidYearMessage { get; init; } = "Please select the year between the minimum and maximum amount";
    public string InvalidMonthMessage { get; init; } = "Select a month";
    public string InvalidDayMessage { get; set; } = "Select a day";
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
