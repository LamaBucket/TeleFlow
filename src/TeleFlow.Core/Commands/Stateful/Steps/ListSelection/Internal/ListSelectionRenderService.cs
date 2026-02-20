using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase.Internal;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;

internal static class ListSelectionRenderService
{
    internal static InlineKeyboardMarkup RenderMarkup<T>(ListSelectionOptions<T> config, Func<CallbackAction, string> codec, ListSelectionCommandStepViewModel<T> vm)
    {
        return config.Mode switch
        {
            ListSelectionMode.SingleSelect<T>    => RenderSingle(config, codec, vm),
            ListSelectionMode.MultiSelect<T>     => RenderMulti(config, codec, vm),
            _ => throw new NotSupportedException($"Unknown mode: {config.Mode.GetType().FullName}")
        };
    }

    private static InlineKeyboardMarkup RenderSingle<T>(ListSelectionOptions<T> config, Func<CallbackAction, string> codec, ListSelectionCommandStepViewModel<T> vm)
    {
        var builder = new InlineKeyboardBuilder();

        AppendItemsGrid(config, codec, vm, builder);
        
        builder.NewRow();
        AppendNavigation(config.ButtonTextOptions, codec, builder);

        return builder.Build();
    }

    private static InlineKeyboardMarkup RenderMulti<T>(ListSelectionOptions<T> config, Func<CallbackAction, string> codec, ListSelectionCommandStepViewModel<T> vm)
    {
        var builder = new InlineKeyboardBuilder();

        AppendItemsGrid(config, codec, vm, builder);

        builder.NewRow();
        AppendNavigation(config.ButtonTextOptions, codec, builder);

        builder.NewRow();
        builder.ButtonCallback(config.ButtonTextOptions.MultiSelectFinishButtonText, codec(CallbackActions.Step.Finish));

        return builder.Build();
    }


    private static void AppendItemsGrid<T>(ListSelectionOptions<T> config, Func<CallbackAction, string> codec, ListSelectionCommandStepViewModel<T> vm, InlineKeyboardBuilder builder)
    {
        var pageConfig = config.PageSizeOptions;

        int pageSize = ListSelectionPagingHelper.CalculatePageSize(pageConfig);

        int pageStartIndex = vm.Page * pageSize;
        int currentItemIndex = pageStartIndex;

        for(int row = 0; row < pageConfig.MaxRowsInPage; row++)
        {
            bool rowHasButtons = false;

            for(int col = 0; col < pageConfig.MaxItemsInRow; col++)
            {
                if(currentItemIndex > vm.Values.Count - 1)
                {
                    if (pageConfig.FitItemsOnLastPage)
                    {
                        builder.ButtonCallback(DefaultButtonTexts.EmptyButtonText, codec(CallbackActions.Ui.Noop));
                        rowHasButtons = true;
                    }
                    else break;
                }
                else
                {
                    var item = vm.Values[currentItemIndex];
                    var text = config.DisplayNameParser(item);

                    if (vm.IsSelected(currentItemIndex))
                        text = $"*{text}*";
                    
                    builder.ButtonCallback(text, codec(CallbackActions.Ui.Select(currentItemIndex)));
                    rowHasButtons = true;
                }

                currentItemIndex += 1;
            }

            if (!pageConfig.FitItemsOnLastPage && !rowHasButtons)
                break;
            
            if (rowHasButtons && row != pageConfig.MaxRowsInPage - 1)
                builder.NewRow();
        }
    }

    private static void AppendNavigation(ListSelectionButtonTextOptions config, Func<CallbackAction, string> codec, InlineKeyboardBuilder b)
    {
        b.ButtonCallback(config.PrevPageButtonText, codec(CallbackActions.Ui.PrevPage));
        b.ButtonCallback(config.NextPageButtonText, codec(CallbackActions.Ui.NextPage));
    }

}