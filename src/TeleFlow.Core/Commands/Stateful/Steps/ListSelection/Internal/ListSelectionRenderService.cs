using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.StepAction;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.UiAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;

internal static class ListSelectionRenderService
{
    internal static InlineKeyboardMarkup RenderMarkup<T>(ListSelectionCommandStepOptions<T> config, Func<CallbackAction, string> markupButtonActionCodec, ListSelectionCommandStepViewModel<T> vm)
    {
        IReadOnlyList<int> SelectedIndexes = vm.SelectedIndexes;

        int cols = config.MaxItemsInRow;
        int rows = config.MaxRowsInPage;
        int pageSize = cols * rows;

        int start = vm.Page * pageSize;
        int count = vm.Values.Count;

        if (start < 0) start = 0;
        if (start >= count && count > 0)
        {
            vm.Page = (count - 1) / pageSize;
            start = vm.Page * pageSize;
        }

        var b = new InlineKeyboardBuilder();

        int idx = start;
        for (int r = 0; r < rows && idx < count; r++)
        {
            for (int c = 0; c < cols && idx < count; c++, idx++)
            {
                T item = vm.Values[idx];
                string text = config.DisplayNameParser(item);

                if (config.Mode is ListSelectionMode<T>.Multi)
                {
                    if(SelectedIndexes.Contains(idx))
                        text = '*' + text + '*';
                }

                b.ButtonCallback(text, markupButtonActionCodec(CallbackActions.Ui.Select(idx)));
            }

            b.NewRow();
        }

        bool hasPrev = vm.Page > 0;
        bool hasNext = (start + pageSize) < count;

        if (hasPrev || hasNext)
        {
            if (hasPrev)
                b.ButtonCallback("<-", markupButtonActionCodec(CallbackActions.Ui.PrevPage));
            if (hasNext)
                b.ButtonCallback("->", markupButtonActionCodec(CallbackActions.Ui.NextPage));
            b.NewRow();
        }

        if(config.Mode is ListSelectionMode<T>.Multi)
            b.ButtonCallback("Done", markupButtonActionCodec(new Finish()));

        return b.Build();
    }
}