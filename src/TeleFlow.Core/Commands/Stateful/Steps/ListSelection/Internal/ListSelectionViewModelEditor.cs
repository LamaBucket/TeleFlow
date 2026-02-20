using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;

internal static class ListSelectionViewModelEditor
{
    public static bool NextPage<T>(ListSelectionCommandStepViewModel<T> vm, ListSelectionPageSizeOptions config)
    {
        if (ListSelectionPagingHelper.CalculateMaxPage(config, vm) <= vm.Page)
            return false;

        vm.Page += 1;
        return true;
    }

    public static bool PrevPage<T>(ListSelectionCommandStepViewModel<T> vm)
    {
        if(vm.Page <= 0)
            return false;
        
        vm.Page -= 1;
        return true;
    }

    public static bool Toggle<T>(ListSelectionCommandStepViewModel<T> vm, int index)
    {
        if(index <= 0 || index >= vm.Values.Count)
            return false;

        vm.Toggle(index);
        return true;
    }
}