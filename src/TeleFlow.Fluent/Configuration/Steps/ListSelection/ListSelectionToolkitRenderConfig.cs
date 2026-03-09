using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;
using TeleFlow.Fluent.Configuration.Steps.Base;

namespace TeleFlow.Fluent.Configuration.Steps.ListSelection;

public class ListSelectionToolkitRenderConfig<T>
{
    public ListSelectionDisplayNameParser<T> DisplayNameParser { get; set; } = ListSelectionDefaults.DisplayNameParser;

    public ListSelectionUserPrompt<T> UserPrompt { get; set; } = ListSelectionDefaults.UserPrompt;

    public bool IsPaddingOn { get; set; } = ListSelectionDefaults.IsPaddingOn;

    public string PrevPageButtonText { get; set; } = ListSelectionDefaults.PrevPageButtonText;
    public string NextPageButtonText { get; set; } = ListSelectionDefaults.NextPageButtonText;
    public string MultiSelectFinishButtonText { get; set; } = ListSelectionDefaults.MultiSelectFinishButtonText;
    public string EmptyButtonText { get; set; } = CallbackButtonDefaultTexts.EmptyButtonText;
}