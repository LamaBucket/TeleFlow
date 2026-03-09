namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public record ListSelectionStepDataConstraints
{
    public required int PageRows { get; init; }

    public required int PageColumns { get; init; }
}