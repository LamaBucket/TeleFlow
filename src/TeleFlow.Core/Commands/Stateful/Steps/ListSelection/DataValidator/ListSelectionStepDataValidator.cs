namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.DataValidator;

internal static class ListSelectionStepDataValidator
{
    public static bool IsValid<T>(ListSelectionStepData<T> data, ListSelectionStepDataConstraints c)
    {
        var valuesCount = data.Values.Count();

        if(data.Page < 0 || data.Page > CalculateMaxPage(c.PageRows, c.PageColumns, valuesCount))
            return false;

        return EnsureSelectedIndexesValid<T>(data.SelectedIndexes, valuesCount);
    }

    private static bool EnsureSelectedIndexesValid<T>(IEnumerable<int> selectedIndexes, int valuesCount)
    {
        foreach(var idx in selectedIndexes)
        {
            if(idx < 0 || idx >= valuesCount)
                return false;
        }

        return true;
    }

    private static int CalculateMaxPage(int pageRows, int pageCols, int itemCount)
    {
        int pageSize = CalculatePageSize(pageRows, pageCols);
        return (itemCount - 1) / pageSize;
    }

    private static int CalculatePageSize(int pageRows, int pageCols) 
            => pageRows * pageCols;
}