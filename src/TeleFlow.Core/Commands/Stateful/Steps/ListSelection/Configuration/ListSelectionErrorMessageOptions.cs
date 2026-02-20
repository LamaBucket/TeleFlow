namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;

public class ListSelectionErrorMessageOptions
{
    public string? IndexOutOfRangeMessage { get; set; } = "Please select an item from the list";

    public string? LastPageMessage { get; init; } = "There is no more items";
    
    public string? FirstPageMessage { get; init; } = "There is no more items";
}