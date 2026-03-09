namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render;

public record DateSelectionRenderServiceOptions
{
    public required Func<int, string> MonthNumberToName { get; init; }
    public required Func<int, string> WeekdayIndexToName { get; init; }
    public required DayOfWeek DaySelectionWeekStart { get; init; }
    
    public required Func<IServiceProvider, DateSelectionStepData, string> UserPrompt { get; init; }
    
    public required string PrevYearPageButtonText { get; init; }     
    public required string NextYearPageButtonText { get; init; } 

    public required string PrevYearItemButtonText { get; init; } 
    public required string NextYearItemButtonText { get; init; } 

    public required string PrevMonthItemButtonText { get; init; }
    public required string NextMonthItemButtonText { get; init; }
    
    public required string EmptyButtonText { get; init; }  

    public required string YearMonthFormatOnDayPage { get; init; }
    
}