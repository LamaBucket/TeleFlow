using System.Globalization;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection;
using TeleFlow.Fluent.Configuration.Base;

namespace TeleFlow.Fluent.Configuration.DateSelection;

internal static class DateSelectionDefaults
{
    public static readonly string LastPageMessage = "There is no more items";
    public static readonly string FirstPageMessage = "There is no more items";
    public static readonly string InvalidYearMessage = "Please select the year between the minimum and maximum amount";
    public static readonly string InvalidMonthMessage = "Select a month";
    public static readonly string InvalidDayMessage = "Select a day";


    public static readonly DateOnly MinDate = DateOnly.MinValue;
    public static readonly DateOnly MaxDate = DateOnly.MaxValue;
    public static readonly int YearSelectionRows = 3;
    public static readonly int YearSelectionColumns = 3;
    public static readonly int MonthSelectionRows = 4;


    public static readonly Func<int, string> MonthNumberToName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName;
    public static readonly Func<int, string> WeekdayIndexToName = index => CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedDayNames[index];
    public static readonly DayOfWeek DaySelectionWeekStart = DayOfWeek.Sunday;
    
    public static readonly Func<IServiceProvider, DateSelectionStepData, string> UserPrompt = DefaultUserPrompt;
    
    public static readonly string PrevYearPageButtonText = CallbackButtonDefaultTexts.PrevPageButtonText;
    public static readonly string NextYearPageButtonText = CallbackButtonDefaultTexts.NextPageButtonText;

    public static readonly string PrevYearItemButtonText = CallbackButtonDefaultTexts.PrevItemButtonText;
    public static readonly string NextYearItemButtonText = CallbackButtonDefaultTexts.NextItemButtonText;

    public static readonly string PrevMonthItemButtonText = CallbackButtonDefaultTexts.PrevItemButtonText;
    public static readonly string NextMonthItemButtonText = CallbackButtonDefaultTexts.NextItemButtonText;

    public static readonly string YearMonthFormatOnDayPage = "MMM yyyy";


    private static string DefaultUserPrompt(IServiceProvider serviceProvider, DateSelectionStepData model)
    {
        var selectedDate = new DateOnly(model.YearSelected, model.MonthSelected, model.DaySelected);
        return model.Page switch
        {
            DateSelectionStepPage.YearSelection => model.DateSelectionCompleted 
                ? $"You selected: {selectedDate:yyyy}"
                : "Please select a year",
            DateSelectionStepPage.MonthSelection => model.DateSelectionCompleted 
                ? $"You selected: {selectedDate:MM/yyyy}"
                : "Please select a month",
            DateSelectionStepPage.DaySelection => model.DateSelectionCompleted 
                ? $"You selected: {selectedDate:MM/dd/yyyy}"
                : "Please select a year",
            _ => throw new Exception($"Not expected page value: {model.Page}")
        };
    }
}