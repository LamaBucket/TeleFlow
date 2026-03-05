namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public class ListSelectionStepDataConstraints
{
    public required int PageRows { get; init; }

    public required int PageColumns { get; init; }
}