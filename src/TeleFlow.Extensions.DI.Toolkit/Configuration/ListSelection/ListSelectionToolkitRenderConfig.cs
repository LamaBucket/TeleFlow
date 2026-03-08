using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;
using TeleFlow.Extensions.DI.Toolkit.Configuration.Base;

namespace TeleFlow.Extensions.DI.Toolkit.Configuration.ListSelection;

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