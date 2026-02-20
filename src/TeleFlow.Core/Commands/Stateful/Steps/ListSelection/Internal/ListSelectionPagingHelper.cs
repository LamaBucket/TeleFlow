using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;

internal static class ListSelectionPagingHelper
{
    public const int MinPage = 0;

    public static int CalculateMaxPage<T>(ListSelectionPageSizeOptions config, ListSelectionCommandStepViewModel<T> vm)
        => CalculateMaxPage(config, vm.Values.Count);

    public static int CalculateMaxPage(ListSelectionPageSizeOptions config, int itemCount)
        => CalculateMaxPage(config.MaxRowsInPage, config.MaxItemsInRow, itemCount);

    public static int CalculateMaxPage(int pageRows, int pageCols, int itemCount)
    {
        int pageSize = CalculatePageSize(pageRows, pageCols);
        return (itemCount - 1) / pageSize;
    }


    public static int CalculatePageSize(ListSelectionPageSizeOptions config)
        => CalculatePageSize(config.MaxRowsInPage, config.MaxItemsInRow);

    public static int CalculatePageSize(int pageRows, int pageCols) 
        => pageRows * pageCols;
}