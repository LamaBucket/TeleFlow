namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed class ListSelectionCommandStepViewModel<T>
{
    public int Page { get; set; }

    public IReadOnlyList<T> Values { get; init; }

    private readonly List<int> _selectedIndexes;
    

    public T SelectedValue 
        => _selectedIndexes.Count == 1 
                ? Values[_selectedIndexes[0]] 
                : throw new InvalidOperationException($"SelectedValue is available only when exactly one item is selected, but {_selectedIndexes.Count} items are selected.");
    
    public IReadOnlyList<T> SelectedValues => Values.Where((val, i) => _selectedIndexes.Contains(i)).ToList();
    public IReadOnlyList<int> SelectedIndexes => _selectedIndexes;

    public bool IsSelected(int index) 
        => _selectedIndexes.Contains(index);


    public void Toggle(int index)
    {
        if (index < 0 || index >= Values.Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be in range [0..{Values.Count - 1}].");

        if (!_selectedIndexes.Remove(index))
            _selectedIndexes.Add(index);
    }


    public ListSelectionCommandStepViewModel(IReadOnlyList<T> values) : this(values, 0, [])
    {
    }

    public ListSelectionCommandStepViewModel(IReadOnlyList<T> values, int page, List<int> selectedIndexes)
    {
        if (values.Count == 0)
            throw new ArgumentException("Values collection cannot be empty.", nameof(values));
        
        ArgumentOutOfRangeException.ThrowIfNegative(page, nameof(page));

        foreach(var idx in selectedIndexes)
        {
            if(idx < 0 || idx >= values.Count)
                throw new ArgumentOutOfRangeException(nameof(selectedIndexes), idx, $"Selected index must be in range [0..{values.Count - 1}].");
        }

        Values = values;
        Page = page;
        _selectedIndexes = selectedIndexes;
    }
}