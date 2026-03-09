using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render;
using TeleFlow.Fluent.Configuration.Base;

namespace TeleFlow.Fluent.Configuration.DateSelection;

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
            EmptyButtonText = Render.EmptyButtonText,
            
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