using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render.Internal;

internal static class YearSelectionRenderService
{
    public static InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec,
                                                    DateSelectionStepData model,
                                                    DateSelectionStepDataConstraints constraints,
                                                    DateSelectionRenderServiceOptions config)
    {
        int rows = constraints.YearSelectionRows;
        int cols = constraints.YearSelectionColumns;

        int pageSize = rows * cols;

        int startYear = model.YearPagePivotValue + (model.YearPageIndex * pageSize);

        InlineKeyboardBuilder builder = new();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int offset = (r * cols) + c;
                int year = startYear + offset;
                
                bool yearValid = year <= constraints.MaxDate.Year && year >= constraints.MinDate.Year;
                
                var action = yearValid ? CallbackActions.Ui.Select(year) : CallbackActions.Ui.Noop;
                
                var text = yearValid ? year.ToString() : " ";
                var data = markupButtonActionCodec(action);
                
                if(year == model.YearSelected)
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

}