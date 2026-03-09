namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;

public delegate string ListSelectionDisplayNameParser<T>(T item);

public delegate string ListSelectionUserPrompt<T>(IServiceProvider sp, ListSelectionDisplayNameParser<T> displayNameParser, ListSelectionStepData<T> data);

public record ListSelectionRenderServiceOptions<T>
{
    public required ListSelectionRenderType RenderType { get; init; }

    public required ListSelectionDisplayNameParser<T> DisplayNameParser { get; init; }

    public required ListSelectionUserPrompt<T> UserPrompt { get; init; }

    public required bool IsPaddingOn { get; init; }

    public required string PrevPageButtonText { get; init; }
    public required string NextPageButtonText { get; init; }
    public required string EmptyButtonText { get; init; }  
    public required string MultiSelectFinishButtonText { get; init; }
}

public enum ListSelectionRenderType
{
    SingleSelect,
    MultiSelect
}