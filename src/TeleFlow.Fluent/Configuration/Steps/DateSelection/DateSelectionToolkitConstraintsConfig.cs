namespace TeleFlow.Fluent.Configuration.Steps.DateSelection;

public class DateSelectionToolkitConstraintsConfig
{
    public DateOnly MinDate { get; set; } = DateSelectionDefaults.MinDate;
    public DateOnly MaxDate { get; set; } = DateSelectionDefaults.MaxDate;

    public int YearSelectionRows { get; set; } = DateSelectionDefaults.YearSelectionRows;
    public int YearSelectionColumns { get; set; } = DateSelectionDefaults.YearSelectionColumns;

    public int MonthSelectionRows { get; set; } = DateSelectionDefaults.MonthSelectionRows;
}