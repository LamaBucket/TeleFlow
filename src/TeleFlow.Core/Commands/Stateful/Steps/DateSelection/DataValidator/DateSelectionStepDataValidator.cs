namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.DataValidator;

internal static class DateSelectionStepDataValidator
{
    public static bool IsValid(DateSelectionStepData data, DateSelectionStepDataConstraints c)
    {
        if (!IsSelectedDateWithinBounds(data, c))
            return false;

        if(data.Page == DateSelectionStepPage.YearSelection)
            return IsYearSelectionPageValid(data, c);
        
        return true;
    }

    private static bool IsSelectedDateWithinBounds(DateSelectionStepData data, DateSelectionStepDataConstraints c)
    {
        try
        {
            var date = new DateOnly(data.YearSelected, data.MonthSelected, data.DaySelected);
            return date >= c.MinDate && date <= c.MaxDate;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsYearSelectionPageValid(DateSelectionStepData data, DateSelectionStepDataConstraints c)
    {
        if (data.YearPagePivotValue < DateOnly.MinValue.Year || data.YearPagePivotValue > DateOnly.MaxValue.Year)
            return false;

        int pageSize = GetYearPageSize(c);
        if (pageSize <= 0)
            return false;

        var (startYear, endYear) = GetYearPageYearRange(data, pageSize);

        int minYear = c.MinDate.Year;
        int maxYear = c.MaxDate.Year;

        if (endYear < minYear || startYear > maxYear)
            return false;

        return true;
    }

    private static int GetYearPageSize(DateSelectionStepDataConstraints c)
        => CheckedMultiply(c.YearSelectionRows, c.YearSelectionColumns);

    private static (int startYear, int endYear) GetYearPageYearRange(DateSelectionStepData data, int pageSize)
    {
        int offset = CheckedMultiply(data.YearPageIndex, pageSize);
        int startYear = CheckedAdd(data.YearPagePivotValue, offset);
        int endYear = CheckedAdd(startYear, pageSize - 1);

        return (startYear, endYear);
    }


    private static int CheckedMultiply(int a, int b)
        => checked(a * b);

    private static int CheckedAdd(int a, int b)
        => checked(a + b);

}