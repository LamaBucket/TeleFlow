namespace TeleFlow.Fluent.Configuration.Steps.ListSelection;

public class ListSelectionToolkitConstraintsConfig<T>
{
    public int PageRows { get; set; } = ListSelectionDefaults.PageRows;

    public int PageCols { get; set; } = ListSelectionDefaults.PageCols;
}