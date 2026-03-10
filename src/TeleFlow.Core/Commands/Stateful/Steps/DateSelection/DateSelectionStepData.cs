using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public sealed record DateSelectionStepData(
    int? YearSelected,
    int? MonthSelected,
    int? DaySelected,
    bool DateSelectionCompleted,
    DateSelectionStepPage Page,
    int YearPageIndex,
    int YearPagePivotValue
) : StepData
{
    private static DateSelectionStepData CreateDefault(int pivotYear)
        => new(
            YearSelected: null,
            MonthSelected: null,
            DaySelected: null,
            DateSelectionCompleted: false,
            Page: DateSelectionStepPage.YearSelection,
            YearPageIndex: 0,
            YearPagePivotValue: pivotYear
        );

    public static DateSelectionStepData CreateDefault()
        => CreateDefault(DateTime.UtcNow.Year);
}

public enum DateSelectionStepPage
{
    YearSelection,
    MonthSelection,
    DaySelection
}