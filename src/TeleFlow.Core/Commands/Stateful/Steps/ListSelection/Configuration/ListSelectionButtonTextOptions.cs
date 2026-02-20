using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase.Internal;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;

public class ListSelectionButtonTextOptions
{
    public string PrevPageButtonText { get; init; } = DefaultButtonTexts.PrevPageButtonText;

    public string NextPageButtonText { get; init; } = DefaultButtonTexts.NextPageButtonText;

    public string MultiSelectFinishButtonText { get; init; } = DefaultButtonTexts.DoneButtonText;
}