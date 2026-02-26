using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase.Internal;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.UiAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateInput.Internal;

internal static class DateInputRenderService
{
    private const int MonthsInYear = 12;
    private const int DaysInWeek = 7;

    internal static InlineKeyboardMarkup RenderMarkup(DateInputCommandStepOptions config,
                                                      Func<CallbackAction, string> markupButtonActionCodec,
                                                      DateInputCommandStepViewModel vm)
    {
        return vm.Page switch 
        {
            DateInputCommandStepPage.YearSelection => RenderYearSelectionPage(config, markupButtonActionCodec, vm),
            DateInputCommandStepPage.MonthSelection => RenderMonthSelectionPage(config, markupButtonActionCodec, vm),
            DateInputCommandStepPage.DateSelection => RenderDaySelectionPage(config, markupButtonActionCodec, vm),
            _ =>  throw new InvalidOperationException($"Unsupported date input page: {vm.Page}. This indicates an invalid internal state.")
        };
    }

    private static InlineKeyboardMarkup RenderYearSelectionPage(DateInputCommandStepOptions config,
                                                                Func<CallbackAction, string> markupButtonActionCodec,
                                                                DateInputCommandStepViewModel vm)
    {
        int rows = config.YearSelectionRows;
        int cols = config.YearSelectionColumns;

        int pageSize = rows * cols;

        int startYear = vm.InitYearDate + (vm.YearPageIndex * pageSize);

        InlineKeyboardBuilder builder = new();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int offset = (r * cols) + c;
                int year = startYear + offset;
                
                bool yearValid = year <= config.MaxYear && year >= config.MinYear;
                
                var action = yearValid ? CallbackActions.Ui.Select(year) : CallbackActions.Ui.Noop;
                
                var text = yearValid ? year.ToString() : " ";
                var data = markupButtonActionCodec(action);
                
                if(year == vm.YearSelected)
                    text = $"*{text}*";

                builder.ButtonCallback(text, data);
            }

            builder.NewRow();
        }

        var navAction = CallbackActions.Ui.PrevPage;
        builder.ButtonCallback(config.PrevYearPageButtonText, markupButtonActionCodec(navAction));

        navAction = CallbackActions.Ui.NextPage;
        builder.ButtonCallback(config.NextYearPageButtonText, markupButtonActionCodec(navAction));

        return builder.Build();
    }

    private static InlineKeyboardMarkup RenderMonthSelectionPage(DateInputCommandStepOptions config,
                                                                 Func<CallbackAction, string> markupButtonActionCodec,
                                                                 DateInputCommandStepViewModel vm)
    {
        int rows = config.MonthSelectionRows;
        int cols = CeilDiv(MonthsInYear, rows);

        var b = new InlineKeyboardBuilder();

        b.ButtonCallback(config.PrevYearItemButtonText, markupButtonActionCodec(CallbackActions.Ui.PrevPage));
        b.ButtonCallback(vm.YearSelected.ToString(), markupButtonActionCodec(CallbackActions.Ui.GoToPage((int)DateInputCommandStepPage.YearSelection)));
        b.ButtonCallback(config.NextYearItemButtonText, markupButtonActionCodec(CallbackActions.Ui.NextPage));
        b.NewRow();

        int totalCells = rows * cols;

        for (int cell = 0; cell < totalCells; cell++)
        {
            int month = cell + 1;

            if (month <= MonthsInYear)
            {
                string text = config.Culture.DateTimeFormat.MonthNames[month - 1];
                if (vm.MonthSelected == month)
                    text = $"*{text}*";

                b.ButtonCallback(text, markupButtonActionCodec(CallbackActions.Ui.Select(month)));
            }
            else
            {
                b.ButtonCallback(DefaultButtonTexts.EmptyButtonText, markupButtonActionCodec(CallbackActions.Ui.Noop));
            }

            if ((cell + 1) % cols == 0 && cell != totalCells - 1)
                b.NewRow();
        }

        return b.Build();
    }

    private static InlineKeyboardMarkup RenderDaySelectionPage(DateInputCommandStepOptions config,
                                                               Func<CallbackAction, string> markupButtonActionCodec,
                                                               DateInputCommandStepViewModel vm)
    {
        var b = new InlineKeyboardBuilder();

        int year = vm.YearSelected;
        int month = vm.MonthSelected;

        var culture = config.Culture;
        
        int daysInMonth = DateTime.DaysInMonth(year, month);

        b.ButtonCallback(config.PrevMonthItemButtonText, markupButtonActionCodec(CallbackActions.Ui.PrevPage));

        string headerText = new DateOnly(year, month, 1).ToString(config.YearMonthFormatOnDayPage, culture);
        b.ButtonCallback(headerText, markupButtonActionCodec(CallbackActions.Ui.GoToPage((int)DateInputCommandStepPage.MonthSelection)));

        b.ButtonCallback(config.NextMonthItemButtonText, markupButtonActionCodec(CallbackActions.Ui.NextPage));
        b.NewRow();
        
        for (int i = 0; i < DaysInWeek; i++)
        {
            string weekDay = culture.DateTimeFormat.AbbreviatedDayNames[i];
            b.ButtonCallback(weekDay, markupButtonActionCodec(CallbackActions.Ui.Noop));
        }
        b.NewRow();

        
        int weekDayOffset = GetWeekDayMondayFirst(new DateOnly(year, month, 1));
        int totalCells = daysInMonth + weekDayOffset;
        int gridRows = CeilDiv(totalCells, DaysInWeek);

        for (int r = 0; r < gridRows; r++)
        {
            for (int c = 0; c < DaysInWeek; c++)
            {
                int cellIndex = r * DaysInWeek + c;
                int day = cellIndex - weekDayOffset + 1;

                if (day < 1 || day > daysInMonth)
                {
                    b.ButtonCallback(DefaultButtonTexts.EmptyButtonText, markupButtonActionCodec(CallbackActions.Ui.Noop));
                    continue;
                }

                string text = day.ToString();
                if (vm.YearSelected == year && vm.MonthSelected == month && vm.DaySelected == day)
                    text = $"*{text}*";

                b.ButtonCallback(text, markupButtonActionCodec(CallbackActions.Ui.Select(day)));
            }

            b.NewRow();
        }

        return b.Build();
    }

    private static int GetWeekDayMondayFirst(DateOnly date)
    {
        int wd = (int)date.DayOfWeek; // Sunday=0..Saturday=6
        wd = wd == 0 ? 7 : wd;        // Sunday -> 7
        return wd - 1;                // Monday -> 0, Sunday -> 6
    }

    private static int CeilDiv(int value, int divisor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(divisor);
        return (value + divisor - 1) / divisor;
    }
}