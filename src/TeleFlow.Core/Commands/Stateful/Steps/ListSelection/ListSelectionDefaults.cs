using System.Text;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public static class ListSelectionDefaults
{
    public static readonly string IndexOutOfRangeMessage = "Please select an item from the list";
    public static readonly string LastPageMessage = "There is no more items";
    public static readonly string FirstPageMessage = "There is no more items";

    public static readonly int PageRows = 5;
    public static readonly int PageCols = 2;


    public static readonly string PrevPageButtonText = CallbackButtonDefaultTexts.PrevPageButtonText;
    public static readonly string NextPageButtonText = CallbackButtonDefaultTexts.NextPageButtonText;
    public static readonly string MultiSelectFinishButtonText = CallbackButtonDefaultTexts.DoneButtonText;
    public static readonly bool IsPaddingOn = true;

    public static string DisplayNameParser<TItem>(TItem item) => item?.ToString() ?? CallbackButtonDefaultTexts.EmptyButtonText;
    public static string UserPrompt<TItem>(IServiceProvider sp, ListSelectionDisplayNameParser<TItem> displayNameParser, ListSelectionStepData<TItem> data)
    {
        var singleItemSelection = data.SelectedIndexes.Count() == 1;
        
        StringBuilder sb = new();
        if(singleItemSelection)
            sb.AppendLine($"Use the buttons below to select an item.");
        else
            sb.AppendLine($"Use the buttons below to select items.");

        if(data.ListSelectionFinished)
            if(singleItemSelection)
                sb.AppendLine($"You Selected: {displayNameParser(data.SelectedValue)}");
            else
                sb.AppendLine($"You Selected: {string.Join(", ", data.SelectedValues.Select(x => displayNameParser(x)))}");

        return sb.ToString();
    }


}