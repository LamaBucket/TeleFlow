namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed class ListSelectionCommandStepViewModel<T>
{
    public int Page { get; set; }

    public IReadOnlyList<T> Values { get; init; }

    private readonly List<int> _selectedIndexes;
    

    public T SelectedValue => _selectedIndexes.Count == 1 ? Values[_selectedIndexes[0]] : throw new Exception("More then 1 item is selected!");
    
    public IReadOnlyList<T> SelectedValues => Values.Where((val, i) => _selectedIndexes.Contains(i)).ToList();
    public IReadOnlyList<int> SelectedIndexes => _selectedIndexes;

    public bool IsSelected(int index) 
        => _selectedIndexes.Contains(index);


    public void Toggle(int index)
    {
        if(index < 0 || index >= Values.Count)
                throw new IndexOutOfRangeException();

        if(_selectedIndexes.Contains(index))
            _selectedIndexes.Remove(index);
        else
            _selectedIndexes.Add(index);
    }


    public ListSelectionCommandStepViewModel(IReadOnlyList<T> values) : this(values, 0, [])
    {
    }

    public ListSelectionCommandStepViewModel(IReadOnlyList<T> values, int page, List<int> selectedIndexes)
    {
        if(values.Count == 0) 
            throw new ArgumentException();
        
        ArgumentOutOfRangeException.ThrowIfNegative(page);

        foreach(var idx in selectedIndexes)
        {
            if(idx < 0 || idx >= values.Count)
                throw new ArgumentOutOfRangeException();
        }

        Values = values;
        Page = page;
        _selectedIndexes = selectedIndexes;
    }
}