using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase.Internal;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;

public class ListSelectionRenderService<T> : IStepRenderService<ListSelectionStepData<T>>
{
    private readonly ListSelectionRenderServiceOptions<T> config;

    private readonly ListSelectionStepDataConstraints _contraints;


    public InlineMarkupMessage Render(IServiceProvider serviceProvider, ListSelectionStepData<T> data)
    {
        if(data.ListSelectionFinished)
            return RenderCompleted(serviceProvider, data);
        
        return RenderIncomplete(serviceProvider, data);
    }

    private InlineMarkupMessage RenderCompleted(IServiceProvider serviceProvider, ListSelectionStepData<T> data)
        => new(){ Text = config.UserPrompt(serviceProvider, config.DisplayNameParser, data), Markup = null};

    private InlineMarkupMessage RenderIncomplete(IServiceProvider serviceProvider, ListSelectionStepData<T> data)
    {
        var callbackCodec = serviceProvider.GetRequiredService<ICallbackCodec>();
        var actionParser  = serviceProvider.GetRequiredService<ICallbackActionParser>();

        var markupButtonActionCodec = (CallbackAction action) =>
        {
            return callbackCodec.EncodeAction(actionParser.Parse(action));
        };

        return new()
        {
            Text = config.UserPrompt(serviceProvider, config.DisplayNameParser, data),
            Markup = RenderMarkup(markupButtonActionCodec, data)
        };
    }

    internal InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> codec, ListSelectionStepData<T> vm)
    {
        return config.RenderType switch
        {
            ListSelectionRenderType.SingleSelect    => RenderSingle(codec, vm),
            ListSelectionRenderType.MultiSelect     => RenderMulti(codec, vm),
            _ => throw new NotSupportedException($"Unsupported list selection mode: {config.RenderType}. Expected {ListSelectionRenderType.SingleSelect} or {ListSelectionRenderType.MultiSelect}.")
        };
    }

    private InlineKeyboardMarkup RenderSingle(Func<CallbackAction, string> codec, ListSelectionStepData<T> vm)
    {
        var builder = new InlineKeyboardBuilder();

        AppendItemsGrid(codec, vm, builder);
        
        builder.NewRow();
        AppendNavigation(codec, builder);

        return builder.Build();
    }

    private InlineKeyboardMarkup RenderMulti(Func<CallbackAction, string> codec, ListSelectionStepData<T> vm)
    {
        var builder = new InlineKeyboardBuilder();

        AppendItemsGrid(codec, vm, builder);

        builder.NewRow();
        AppendNavigation(codec, builder);

        builder.NewRow();
        builder.ButtonCallback(config.MultiSelectFinishButtonText, codec(CallbackActions.Step.Finish));

        return builder.Build();
    }


    private void AppendItemsGrid(Func<CallbackAction, string> codec, ListSelectionStepData<T> vm, InlineKeyboardBuilder builder)
    {
        int rows = _contraints.PageRows;
        int cols = _contraints.PageColumns;
        int pageSize = rows * cols;

        int pageStartIndex = vm.Page * pageSize;
        int currentItemIndex = pageStartIndex;

        int valuesCount = vm.Values.Count();

        for(int row = 0; row < rows; row++)
        {
            bool rowHasButtons = false;

            for(int col = 0; col < cols; col++)
            {
                if(currentItemIndex > valuesCount - 1)
                {
                    if (config.IsPaddingOn)
                    {
                        builder.ButtonCallback(DefaultButtonTexts.EmptyButtonText, codec(CallbackActions.Ui.Noop));
                        rowHasButtons = true;
                    }
                    else break;
                }
                else
                {
                    var item = vm.Values.ElementAt(currentItemIndex);
                    var text = config.DisplayNameParser(item);

                    if (vm.IsSelected(currentItemIndex))
                        text = $"*{text}*";
                    
                    builder.ButtonCallback(text, codec(CallbackActions.Ui.Select(currentItemIndex)));
                    rowHasButtons = true;
                }

                currentItemIndex += 1;
            }

            if (!config.IsPaddingOn && !rowHasButtons)
                break;
            
            if (rowHasButtons && row != rows - 1)
                builder.NewRow();
        }
    }

    private void AppendNavigation(Func<CallbackAction, string> codec, InlineKeyboardBuilder b)
    {
        b.ButtonCallback(config.PrevPageButtonText, codec(CallbackActions.Ui.PrevPage));
        b.ButtonCallback(config.NextPageButtonText, codec(CallbackActions.Ui.NextPage));
    }

    public ListSelectionRenderService(ListSelectionRenderServiceOptions<T> config, ListSelectionStepDataConstraints contraints)
    {
        this.config = config;
        _contraints = contraints;
    }
}