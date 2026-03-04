using System.Globalization;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase.Internal;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render;

public class DateSelectionRenderServiceOptions
{
    public Func<int, string> MonthNumberToName { get; init; } = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName;
    public Func<int, string> WeekdayIndexToName { get; init; } = index => CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedDayNames[index];
    public DayOfWeek DaySelectionWeekStart { get; init; } = DayOfWeek.Sunday;
    
    public Func<IServiceProvider, DateSelectionStepViewModel, string> UserPrompt { get; init; } = DefaultUserPrompt;
    
    public string PrevYearPageButtonText { get; init; } = DefaultButtonTexts.PrevPageButtonText;
    public string NextYearPageButtonText { get; init; } = DefaultButtonTexts.NextPageButtonText;

    public string PrevYearItemButtonText { get; init; } = DefaultButtonTexts.PrevItemButtonText;
    public string NextYearItemButtonText { get; init; } = DefaultButtonTexts.NextItemButtonText;

    public string PrevMonthItemButtonText { get; init; } = DefaultButtonTexts.PrevItemButtonText;
    public string NextMonthItemButtonText { get; init; } = DefaultButtonTexts.NextItemButtonText;

    public string YearMonthFormatOnDayPage { get; init; } = "MMM yyyy";


    private static string DefaultUserPrompt(IServiceProvider serviceProvider, DateSelectionStepViewModel model)
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