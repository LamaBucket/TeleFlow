namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public class DateSelectionStepDataConstraints
{
    public required DateOnly MinDate { get; init; }
    public required DateOnly MaxDate { get; init; }

    public required int YearSelectionRows { get; init; }
    public required int YearSelectionColumns { get; init; }

    public required int MonthSelectionRows { get; init; }
}