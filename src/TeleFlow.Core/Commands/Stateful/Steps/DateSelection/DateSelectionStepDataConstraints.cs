namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public class DateSelectionStepDataConstraints
{
    public DateOnly MinDate { get; init; } = DateOnly.MinValue;
    public DateOnly MaxDate { get; init; } = DateOnly.MaxValue;

    public int YearSelectionRows { get; init; } = 3;
    public int YearSelectionColumns { get; init; } = 3;

    public int MonthSelectionRows { get; init; } = 4;
}