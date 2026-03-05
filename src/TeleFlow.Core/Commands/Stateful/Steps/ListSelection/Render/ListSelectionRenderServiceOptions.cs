using System.Text;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase.Internal;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;

public delegate string ListSelectionDisplayNameParser<T>(T item);

public delegate string ListSelectionUserPrompt<T>(IServiceProvider sp, ListSelectionDisplayNameParser<T> displayNameParser, ListSelectionStepData<T> data);

public class ListSelectionRenderServiceOptions<T>
{
    public required ListSelectionRenderType RenderType { get; init; }

    public ListSelectionDisplayNameParser<T> DisplayNameParser { get; init; } = DefaultDisplayNameParser;

    public ListSelectionUserPrompt<T> UserPrompt { get; init; } = DefaultUserPrompt;

    public bool IsPaddingOn { get; init; } = true;

    public string PrevPageButtonText { get; init; } = DefaultButtonTexts.PrevPageButtonText;

    public string NextPageButtonText { get; init; } = DefaultButtonTexts.NextPageButtonText;

    public string MultiSelectFinishButtonText { get; init; } = DefaultButtonTexts.DoneButtonText;

    private static string DefaultDisplayNameParser<TItem>(TItem item)
        =>  item?.ToString() ?? DefaultButtonTexts.EmptyButtonText;

    private static string DefaultUserPrompt<TItem>(IServiceProvider sp, ListSelectionDisplayNameParser<TItem> displayNameParser, ListSelectionStepData<TItem> data)
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

public enum ListSelectionRenderType
{
    SingleSelect,
    MultiSelect
}