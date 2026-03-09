namespace TeleFlow.Fluent.Configuration.Steps.DateSelection;

public class DateSelectionToolkitConstraintsConfig
{
    public DateOnly MinDate { get; init; } = DateSelectionDefaults.MinDate;
    public DateOnly MaxDate { get; init; } = DateSelectionDefaults.MaxDate;

    public int YearSelectionRows { get; init; } = DateSelectionDefaults.YearSelectionRows;
    public int YearSelectionColumns { get; init; } = DateSelectionDefaults.YearSelectionColumns;

    public int MonthSelectionRows { get; init; } = DateSelectionDefaults.MonthSelectionRows;
}