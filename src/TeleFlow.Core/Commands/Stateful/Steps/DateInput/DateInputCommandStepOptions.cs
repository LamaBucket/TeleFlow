using System.Globalization;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase.Internal;

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


    public string PrevYearPageButtonText { get; init; } = DefaultButtonTexts.PrevPageButtonText;
    public string NextYearPageButtonText { get; init; } = DefaultButtonTexts.NextPageButtonText;

    public string PrevYearItemButtonText { get; init; } = DefaultButtonTexts.PrevItemButtonText;
    public string NextYearItemButtonText { get; init; } = DefaultButtonTexts.NextItemButtonText;

    public string PrevMonthItemButtonText { get; init; } = DefaultButtonTexts.PrevItemButtonText;
    public string NextMonthItemButtonText { get; init; } = DefaultButtonTexts.NextItemButtonText;

    public string YearMonthFormatOnDayPage { get; init; } = "MMM yyyy";

    public string? LastPageMessage { get; init; } = "There is no more items";
    public string? FirstPageMessage { get; init; } = "There is no more items";

    public string InvalidYearMessage { get; init; } = "Please select the year between the minimum and maximum amount";
    public string InvalidMonthMessage { get; init; } = "Select a month";
    public string InvalidDayMessage { get; set; } = "Select a day";
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
