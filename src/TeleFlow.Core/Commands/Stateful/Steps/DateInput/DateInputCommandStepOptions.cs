using System.Globalization;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateInput;

public class DateInputCommandStepOptions
{
    public required DateInputMode Mode { get; init; }


    public required CultureInfo Culture { get; init; }


    public int MinYear { get; init; } = DateOnly.MinValue.Year;

    public int MaxYear { get; init; } = DateOnly.MaxValue.Year;


    public int YearSelectionRows { get; init; } = 3;

    public int YearSelectionColumns { get; init; } = 3;

    public int MonthSelectionRows { get; init; } = 4;
}


public abstract class DateInputMode
{
    public sealed class YearOnly : DateInputMode
    {
        public required Func<CommandStepCommitContext, int, Task> OnCommit { get; init; }
    }

    public sealed class YearMonth : DateInputMode
    {
        public required Func<CommandStepCommitContext, (int year, int month), Task> OnCommit { get; init; }
    }

    public sealed class YearMonthDay : DateInputMode
    {
        public required Func<CommandStepCommitContext, DateOnly, Task> OnCommit { get; init; }
    }
}
