using TeleFlow.Core.Commands.Stateful.Steps.DateSelection;
using TeleFlow.Fluent.Configuration.Steps.Base;

namespace TeleFlow.Fluent.Configuration.Steps.DateSelection;

public class DateSelectionToolkitRenderConfig
{
    public Func<int, string> MonthNumberToName { get; set; } = DateSelectionDefaults.MonthNumberToName;
    public Func<int, string> WeekdayIndexToName { get; set; } = DateSelectionDefaults.WeekdayIndexToName;
    public DayOfWeek DaySelectionWeekStart { get; set; } = DateSelectionDefaults.DaySelectionWeekStart;
    
    public Func<IServiceProvider, DateSelectionStepData, string> UserPrompt { get; set; } = DateSelectionDefaults.UserPrompt;
    
    public string PrevYearPageButtonText { get; set; } = DateSelectionDefaults.PrevYearPageButtonText;
    public string NextYearPageButtonText { get; set; } = DateSelectionDefaults.NextYearPageButtonText; 

    public string PrevYearItemButtonText { get; set; } = DateSelectionDefaults.PrevYearItemButtonText; 
    public string NextYearItemButtonText { get; set; } = DateSelectionDefaults.NextYearItemButtonText; 

    public string PrevMonthItemButtonText { get; set; } = DateSelectionDefaults.PrevMonthItemButtonText;
    public string NextMonthItemButtonText { get; set; } = DateSelectionDefaults.NextMonthItemButtonText;

    public string EmptyButtonText { get; set; } = CallbackButtonDefaultTexts.EmptyButtonText;

    public string YearMonthFormatOnDayPage { get; set; } = DateSelectionDefaults.YearMonthFormatOnDayPage;
}