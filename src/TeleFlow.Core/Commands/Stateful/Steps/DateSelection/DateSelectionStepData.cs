using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public sealed record DateSelectionStepData(
    int YearSelected,
    int MonthSelected,
    int DaySelected,
    bool DateSelectionCompleted,
    DateSelectionStepPage Page,
    int YearPageIndex,
    int YearPagePivotValue
) : StepData
{
    public static DateSelectionStepData CreateDefault(DateTime utcNow)
        => new(
            YearSelected: utcNow.Year,
            MonthSelected: utcNow.Month,
            DaySelected: utcNow.Day,
            DateSelectionCompleted: false,
            Page: DateSelectionStepPage.YearSelection,
            YearPageIndex: 0,
            YearPagePivotValue: utcNow.Year
        );

    public static DateSelectionStepData CreateDefault()
        => CreateDefault(DateTime.UtcNow);
}

public enum DateSelectionStepPage
{
    YearSelection,
    MonthSelection,
    DaySelection
}