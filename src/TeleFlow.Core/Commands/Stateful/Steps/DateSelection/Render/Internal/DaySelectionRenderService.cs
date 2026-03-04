using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase.Internal;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render.Internal;

internal static class DaySelectionRenderService
{
    private const int DaysInWeek = 7;

    public static InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec,
                                                               DateSelectionStepViewModel model,
                                                               DateSelectionStepVMConstraints constraints,
                                                               DateSelectionRenderServiceOptions config)
    {
        var b = new InlineKeyboardBuilder();

        AppendHeaderControls(b, markupButtonActionCodec, model, config);
        b.NewRow();

        AppendWeekDayNamesRow(b, markupButtonActionCodec, config);
        b.NewRow();

        AppendDaySelectionButtons(b, markupButtonActionCodec, model, constraints, config);

        return b.Build();
    }

    private static void AppendHeaderControls(InlineKeyboardBuilder b, Func<CallbackAction, string> markupButtonActionCodec, DateSelectionStepViewModel model, DateSelectionRenderServiceOptions config)
    {
        b.ButtonCallback(config.PrevMonthItemButtonText, markupButtonActionCodec(CallbackActions.Ui.PrevPage));

        string headerText = new DateOnly(model.YearSelected, model.MonthSelected, 1).ToString(config.YearMonthFormatOnDayPage);
        b.ButtonCallback(headerText, markupButtonActionCodec(CallbackActions.Ui.GoToPage((int)DateSelectionStepPage.MonthSelection)));

        b.ButtonCallback(config.NextMonthItemButtonText, markupButtonActionCodec(CallbackActions.Ui.NextPage));
    }

    private static void AppendWeekDayNamesRow(InlineKeyboardBuilder b, Func<CallbackAction, string> markupButtonActionCodec, DateSelectionRenderServiceOptions config)
    {
        for (int i = 0; i < DaysInWeek; i++)
        {
            string weekDay = config.WeekdayIndexToName(i);
            b.ButtonCallback(weekDay, markupButtonActionCodec(CallbackActions.Ui.Noop));
        }
    }

    private static void AppendDaySelectionButtons(InlineKeyboardBuilder b, Func<CallbackAction, string> markupButtonActionCodec, DateSelectionStepViewModel model, DateSelectionStepVMConstraints constraints, DateSelectionRenderServiceOptions config)
    {        
        int year = model.YearSelected;
        int month = model.MonthSelected;
        
        int daysInMonth = DateTime.DaysInMonth(year, month);
        
        int weekDayOffset = GetWeekDayOffset(new DateOnly(year, month, 1), (int)config.DaySelectionWeekStart);
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

                var date = new DateOnly(year, month, day);
                if (date < constraints.MinDate || date > constraints.MaxDate)
                {
                    b.ButtonCallback(DefaultButtonTexts.EmptyButtonText, markupButtonActionCodec(CallbackActions.Ui.Noop));
                    continue;
                }

                string text = day.ToString();
                if (model.DaySelected == day)
                    text = $"*{text}*";

                b.ButtonCallback(text, markupButtonActionCodec(CallbackActions.Ui.Select(day)));
            }

            b.NewRow();
        }
    }

    private static int GetWeekDayOffset(DateOnly date, int weekStartIndex)
    {
        if (weekStartIndex < 0 || weekStartIndex > 6)
            throw new ArgumentOutOfRangeException(nameof(weekStartIndex), "WeekStartIndex must be in range 0..6 (Sunday..Saturday).");

        int dayIndex = (int)date.DayOfWeek;
        int offset = (dayIndex - weekStartIndex + 7) % 7;
        return offset;
    }

    private static int CeilDiv(int value, int divisor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(divisor);
        return (value + divisor - 1) / divisor;
    }
}