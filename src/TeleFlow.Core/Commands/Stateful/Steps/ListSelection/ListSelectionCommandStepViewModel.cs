namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed class ListSelectionCommandStepViewModel<T>
{
    public IReadOnlyList<T> Values { get; init; }

    public int Page { get; set; }
    
    public IReadOnlyList<T> SelectedValues => Values.Where((val, i) => _selectedIndexes.Contains(i)).ToList();

    public IReadOnlyList<int> SelectedIndexes => _selectedIndexes;


    private readonly List<int> _selectedIndexes;


    public void Toggle(int index)
    {
        if(_selectedIndexes.Contains(index))
            _selectedIndexes.Remove(index);
        else
            _selectedIndexes.Add(index);
    }

    public ListSelectionCommandStepViewModel(IReadOnlyList<T> values, int page = 0, List<int>? selectedIndexes = null)
    {
        Values = values;
        Page = page;
        _selectedIndexes = selectedIndexes ?? [];
    }
}