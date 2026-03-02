namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelectionCommandStep.Internal;

internal static class DateSelectionViewModelValidator
{
    public static bool IsValid(DateSelectionStepViewModel vm, DateSelectionStepVMConstraints c)
    {
        if (!IsSelectedDateWithinBounds(vm, c))
            return false;

        if(vm.Page == DateSelectionStepPage.YearSelection)
            return IsYearSelectionStateValid(vm, c);
        
        return true;
    }

    private static bool IsSelectedDateWithinBounds(DateSelectionStepViewModel vm, DateSelectionStepVMConstraints c)
    {
        try
        {
            var date = new DateOnly(vm.YearSelected, vm.MonthSelected, vm.DaySelected);
            return date >= c.MinDate && date <= c.MaxDate;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsYearSelectionStateValid(DateSelectionStepViewModel vm, DateSelectionStepVMConstraints c)
    {
        if (vm.YearPagePivotValue < DateOnly.MinValue.Year || vm.YearPagePivotValue > DateOnly.MaxValue.Year)
            return false;

        int pageSize = GetYearPageSize(c);
        if (pageSize <= 0)
            return false;

        var (startYear, endYear) = GetYearPageYearRange(vm, pageSize);

        int minYear = c.MinDate.Year;
        int maxYear = c.MaxDate.Year;

        if (endYear < minYear || startYear > maxYear)
            return false;

        return true;
    }

    private static int GetYearPageSize(DateSelectionStepVMConstraints c)
        => CheckedMultiply(c.YearSelectionRows, c.YearSelectionColumns);

    private static (int startYear, int endYear) GetYearPageYearRange(DateSelectionStepViewModel vm, int pageSize)
    {
        int offset = CheckedMultiply(vm.YearPageIndex, pageSize);
        int startYear = CheckedAdd(vm.YearPagePivotValue, offset);
        int endYear = CheckedAdd(startYear, pageSize - 1);

        return (startYear, endYear);
    }


    private static int CheckedMultiply(int a, int b)
        => checked(a * b);

    private static int CheckedAdd(int a, int b)
        => checked(a + b);

}