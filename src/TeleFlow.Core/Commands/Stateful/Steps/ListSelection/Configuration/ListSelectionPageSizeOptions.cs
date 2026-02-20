namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;

public class ListSelectionPageSizeOptions
{
    public int MaxItemsInRow { get; init; } = 2;

    public int MaxRowsInPage { get; init; } = 5;
    
    public bool FitItemsOnLastPage { get; set; } = false;
}