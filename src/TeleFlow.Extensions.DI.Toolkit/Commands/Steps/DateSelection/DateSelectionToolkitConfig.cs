using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render;
using TeleFlow.Extensions.DI.Toolkit.Commands.Steps.BaseConfigs;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.DateSelection;

public class DateSelectionToolkitConfig : CallbackStepToolkitConfig
{
    public DateSelectionToolkitRenderConfig Render { get; init; } = new();

    public DateSelectionToolkitConstraintsConfig Constraints { get; init; } = new();

    public string? LastPageMessage { get; set; } = DateSelectionDefaults.LastPageMessage;
    public string? FirstPageMessage { get; set; } = DateSelectionDefaults.FirstPageMessage;
    
    public string? InvalidYearMessage { get; set; } = DateSelectionDefaults.InvalidYearMessage;
    public string? InvalidMonthMessage { get; set; } = DateSelectionDefaults.InvalidMonthMessage;
    public string? InvalidDayMessage { get; set; } = DateSelectionDefaults.InvalidDayMessage;


    public DateSelectionStepDataConstraints BuildConstraints()
    {
       return new() 
        {
            MinDate = Constraints.MinDate,
            MaxDate = Constraints.MaxDate,
            YearSelectionRows = Constraints.YearSelectionRows,
            YearSelectionColumns = Constraints.YearSelectionColumns,
            MonthSelectionRows = Constraints.MonthSelectionRows
        };
    } 
    public DateSelectionRenderServiceOptions BuildRenderOptions()
    {
        return new()
        {
            MonthNumberToName = Render.MonthNumberToName,
            WeekdayIndexToName = Render.WeekdayIndexToName,
            DaySelectionWeekStart = Render.DaySelectionWeekStart,

            UserPrompt = Render.UserPrompt,
            
            PrevYearPageButtonText = Render.PrevYearPageButtonText,
            NextYearPageButtonText = Render.NextYearPageButtonText,
            
            PrevYearItemButtonText = Render.PrevYearItemButtonText,
            NextYearItemButtonText = Render.NextYearItemButtonText,
            
            PrevMonthItemButtonText = Render.PrevMonthItemButtonText,
            NextMonthItemButtonText = Render.NextMonthItemButtonText,
            
            YearMonthFormatOnDayPage = Render.YearMonthFormatOnDayPage

        };
    }
    public DateSelectionStepOptions BuildStepOptions(IStepRenderService<DateSelectionStepData> renderService, DateSelectionMode mode)
    {
        return new()
        {
            BaseOptions = BuildCallbackOptions(renderService),
            Mode = mode,
            LastPageMessage = LastPageMessage,
            FirstPageMessage = FirstPageMessage,
            
            InvalidYearMessage = InvalidYearMessage,
            InvalidMonthMessage = InvalidMonthMessage,
            InvalidDayMessage = InvalidDayMessage
        };
    }
}

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

    public string YearMonthFormatOnDayPage { get; set; } = DateSelectionDefaults.YearMonthFormatOnDayPage;
}

public class DateSelectionToolkitConstraintsConfig
{
    public DateOnly MinDate { get; init; } = DateSelectionDefaults.MinDate;
    public DateOnly MaxDate { get; init; } = DateSelectionDefaults.MaxDate;

    public int YearSelectionRows { get; init; } = DateSelectionDefaults.YearSelectionRows;
    public int YearSelectionColumns { get; init; } = DateSelectionDefaults.YearSelectionColumns;

    public int MonthSelectionRows { get; init; } = DateSelectionDefaults.MonthSelectionRows;
}