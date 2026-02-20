using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase.Internal;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;

internal static class ListSelectionRenderService
{
    internal static InlineKeyboardMarkup RenderMarkup<T>(ListSelectionCommandStepOptions<T> config,
                                                         Func<CallbackAction, string> codec,
                                                         ListSelectionCommandStepViewModel<T> vm)
    {
        return config.Mode switch
        {
            ListSelectionMode.SingleSelect<T>    => RenderSingle(config, codec, vm),
            ListSelectionMode.MultiSelect<T> ms  => RenderMulti(config, ms.FinishButtonText, codec, vm),
            _ => throw new NotSupportedException($"Unknown mode: {config.Mode.GetType().FullName}")
        };
    }

    #region SingleSelect
    
    private static InlineKeyboardMarkup RenderSingle<T>(ListSelectionCommandStepOptions<T> config,
                                                        Func<CallbackAction, string> codec,
                                                        ListSelectionCommandStepViewModel<T> vm)
    {
        var builder = new InlineKeyboardBuilder();

        AppendItemsGrid(config, codec, vm, builder);
        AppendNavigation(config, codec, builder);

        return builder.Build();
    }
    
    #endregion

    #region MultiSelect

    private static InlineKeyboardMarkup RenderMulti<T>(ListSelectionCommandStepOptions<T> config,
                                                       string finishButtonText,
                                                       Func<CallbackAction, string> codec,
                                                       ListSelectionCommandStepViewModel<T> vm)
    {
        var builder = new InlineKeyboardBuilder();

        AppendItemsGrid(config, codec, vm, builder);
        AppendNavigation(config, codec, builder);

        builder.NewRow();
        builder.ButtonCallback(finishButtonText, codec(CallbackActions.Step.Finish));

        return builder.Build();
    }

    #endregion

    private static void AppendItemsGrid<T>(ListSelectionCommandStepOptions<T> config,
                                           Func<CallbackAction, string> codec,
                                           ListSelectionCommandStepViewModel<T> vm,
                                           InlineKeyboardBuilder b)
    {
        int cols = config.MaxItemsInRow;
        int rows = config.MaxRowsInPage;
        int pageSize = cols * rows;

        int pageStart = vm.Page * pageSize;
        int itemsCount = vm.Values.Count;

        if (pageStart < 0) pageStart = 0;
        if (pageStart > itemsCount) pageStart = itemsCount;

        int remaining = itemsCount - pageStart;
        int take = Math.Min(pageSize, remaining);

        bool isLastPage = (pageStart + take) >= itemsCount;
        bool pad = config.FitItemsOnLastPage && isLastPage;

        for (int i = 0; i < take; i++)
        {
            int idx = pageStart + i;
            var item = vm.Values[idx];

            string text = config.DisplayNameParser(item);
            if (IsSelected(vm, idx))
                text = $"*{text}*";

            b.ButtonCallback(text, codec(CallbackActions.Ui.Select(idx)));

            if ((i + 1) % cols == 0 && i != take - 1)
                b.NewRow();
        }

        if (pad)
        {
            int cellsOnPage = take;
            int totalCells = pageSize;
            int padCount = totalCells - cellsOnPage;

            if (padCount > 0)
            {
                for (int p = 0; p < padCount; p++)
                {
                    int posInRow = (cellsOnPage + p) % cols;
                    if (cellsOnPage > 0 && posInRow == 0)
                        b.NewRow();

                    b.ButtonCallback(DefaultButtonTexts.EmptyButtonText, codec(CallbackActions.Ui.Noop));
                }
            }
        }
    }

    private static void AppendNavigation<T>(ListSelectionCommandStepOptions<T> config,
                                            Func<CallbackAction, string> codec,
                                            InlineKeyboardBuilder b)
    {
        b.NewRow();
        b.ButtonCallback(config.PrevPageButtonText, codec(CallbackActions.Ui.PrevPage));
        b.ButtonCallback(config.NextPageButtonText, codec(CallbackActions.Ui.NextPage));
    }

    private static bool IsSelected<T>(ListSelectionCommandStepViewModel<T> vm, int idx)
    {
        return vm.SelectedIndexes.Contains(idx);
    }

}