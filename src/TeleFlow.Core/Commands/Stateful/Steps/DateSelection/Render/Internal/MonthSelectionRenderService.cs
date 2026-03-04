using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase.Internal;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render.Internal;

internal static class MonthSelectionRenderService
{
    private const int MonthsInYear = 12;

    public static InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec,
                                                     DateSelectionStepData model,
                                                     DateSelectionStepDataConstraints constraints,
                                                     DateSelectionRenderServiceOptions config)
    {
        int rows = constraints.MonthSelectionRows;
        int cols = CeilDiv(MonthsInYear, rows);

        var b = new InlineKeyboardBuilder();

        b.ButtonCallback(config.PrevYearItemButtonText, markupButtonActionCodec(CallbackActions.Ui.PrevPage));
        b.ButtonCallback(model.YearSelected.ToString(), markupButtonActionCodec(CallbackActions.Ui.GoToPage((int)DateSelectionStepPage.YearSelection)));
        b.ButtonCallback(config.NextYearItemButtonText, markupButtonActionCodec(CallbackActions.Ui.NextPage));
        b.NewRow();

        int totalCells = rows * cols;

        for (int cell = 0; cell < totalCells; cell++)
        {
            int month = cell + 1;

            if (constraints.MinDate.Month <= month && month <= constraints.MaxDate.Month)
            {
                string text = config.MonthNumberToName(month);
                if (model.MonthSelected == month)
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

    private static int CeilDiv(int value, int divisor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(divisor);
        return (value + divisor - 1) / divisor;
    }
}