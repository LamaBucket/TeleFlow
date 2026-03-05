using Microsoft.AspNetCore.Mvc.Localization;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public sealed record ListSelectionStepData<T> : StepData
{
    public static ListSelectionStepData<T> CreateDefaultData(IEnumerable<T> values) 
        => new(values, 0, []);


    public int Page { get; init; }

    public IEnumerable<T> Values { get; init; }

    public IEnumerable<int> SelectedIndexes { get; init; }

    public bool ListSelectionFinished { get; init; }
    

    public IEnumerable<T> SelectedValues => Values.Where((val, i) => SelectedIndexes.Contains(i)).ToList();

    public T SelectedValue 
        => SelectedIndexes.Any() 
                ? Values.ElementAt(SelectedIndexes.First())
                : throw new InvalidOperationException($"SelectedValue is available only when exactly one item is selected, but {SelectedIndexes.Count()} items are selected.");

    public bool IsSelected(int index) 
        => SelectedIndexes.Contains(index);

    public ListSelectionStepData<T> Toggle(int index)
    {
        if(SelectedIndexes.Contains(index))
            return this with { SelectedIndexes = SelectedIndexes.Where(i => i != index).ToList() };
        else
            return this with { SelectedIndexes = SelectedIndexes.Append(index) };
    }

    public ListSelectionStepData(IEnumerable<T> values, int page, IEnumerable<int> selectedIndexes)
    {
        if (!values.Any())
            throw new ArgumentException("Values collection cannot be empty.", nameof(values));
        
        ArgumentOutOfRangeException.ThrowIfNegative(page, nameof(page));

        var valuesCount = values.Count();

        foreach(var idx in selectedIndexes)
        {
            if(idx < 0 || idx >= valuesCount)
                throw new ArgumentOutOfRangeException(nameof(selectedIndexes), idx, $"Selected index must be in range [0..{valuesCount - 1}].");
        }

        Values = values;
        Page = page;
        SelectedIndexes = selectedIndexes;
    }
}