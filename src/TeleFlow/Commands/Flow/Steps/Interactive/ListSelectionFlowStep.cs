using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Callbacks;
using TeleFlow.Abstractions.Interactivity;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Flow.Steps.Interactive.Options;
using TeleFlow.Presentation.Builders;
using TeleFlow.Presentation.ViewModels;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Abstractions.Callbacks.CallbackAction.Step;
using static TeleFlow.Abstractions.Callbacks.CallbackAction.Ui;

namespace TeleFlow.Commands.Flow.Steps.Interactive;

public class ListSelectionFlowStep<T> : InteractiveFlowStepBase<ListSelectionStepViewModel<T>>
{
    private readonly ListSelectionStepOptions<T> _options;
    
    protected override async Task<ListSelectionStepViewModel<T>> CreateDefaultViewModel(IServiceProvider sp)
    {
        var values = await _options.ValueProvider.Invoke(sp);
        return new(values);
    }

    protected override InlineKeyboardMarkup RenderMarkup(ICallbackCodec markupEncoder, ListSelectionStepViewModel<T> vm)
    {
        IReadOnlyList<int> SelectedIndexes = vm.SelectedIndexes;

        int cols = _options.MaxItemsInRow;
        int rows = _options.MaxRowsInPage;
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
                string text = _options.DisplayNameParser(item);

                if (_options.Mode is ListSelectionMode<T>.Multi)
                {
                    if(SelectedIndexes.Contains(idx))
                        text = '*' + text + '*';
                }

                string data = markupEncoder.EncodeAction(new ToggleIndex(idx));

                b.ButtonCallback(text, data);
            }

            b.NewRow();
        }

        bool hasPrev = vm.Page > 0;
        bool hasNext = (start + pageSize) < count;

        if (hasPrev || hasNext)
        {
            if (hasPrev)
                b.ButtonCallback("<-", markupEncoder.EncodeAction(new PrevPage()));
            if (hasNext)
                b.ButtonCallback("->", markupEncoder.EncodeAction(new NextPage()));
            b.NewRow();
        }

        if(_options.Mode is ListSelectionMode<T>.Multi)
            b.ButtonCallback("Done", markupEncoder.EncodeAction(new Finish()));

        return b.Build();
    }

    protected override async Task<FlowStepResult> HandleAction(IServiceProvider sp, InteractiveState<ListSelectionStepViewModel<T>> state, CallbackAction action)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var vm = state.ViewModel;

        switch (action)
        {
            case ToggleIndex toggle:
                return await HandleToggleAsync(sp, state, toggle.Index);

            case NextPage:
                return await HandleNextPageAsync(sp, state);

            case PrevPage:
                return await HandlePrevPageAsync(sp, state);

            case Finish:
                return await HandleFinishAsync(sp, state);

            default:
                return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, "Unsupported action.");
        }
    }

    private async Task<FlowStepResult> HandleToggleAsync(IServiceProvider sp, InteractiveState<ListSelectionStepViewModel<T>> state, int index)
    {
        var vm = state.ViewModel;

        if (index < 0 || index >= vm.Values.Count)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, "Invalid item.");

        if (_options.Mode is ListSelectionMode<T>.Single singleSelectMode)
        {
            await singleSelectMode.OnCommit(new(sp), vm.Values[index]);
            await FinalizeStep(sp);
            return FlowStepResult.Next;
        }
        else if (_options.Mode is ListSelectionMode<T>.Multi)
        {
            vm.Toggle(index);
            await UpsertAndRerender(sp, state);
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.Other);
        }

        throw new Exception("Mode Unknown!");
    }

    private async Task<FlowStepResult> HandleNextPageAsync(IServiceProvider sp, InteractiveState<ListSelectionStepViewModel<T>> state)
    {
        var vm = state.ViewModel;

        int pageSize = _options.MaxItemsInRow * _options.MaxRowsInPage;
        int maxPage = vm.Values.Count == 0 ? 0 : (vm.Values.Count - 1) / pageSize;

        if (vm.Page < maxPage) vm.Page++;

        await UpsertAndRerender(sp, state);
        return FlowStepResult.HoldOn(FlowStepHoldOnReason.Other);
    }

    private async Task<FlowStepResult> HandlePrevPageAsync(IServiceProvider sp, InteractiveState<ListSelectionStepViewModel<T>> state)
    {
        var vm = state.ViewModel;

        if (vm.Page > 0) vm.Page--;

        await UpsertAndRerender(sp, state);
        return FlowStepResult.HoldOn(FlowStepHoldOnReason.Other);
    }

    private async Task<FlowStepResult> HandleFinishAsync(IServiceProvider sp, InteractiveState<ListSelectionStepViewModel<T>> state)
    {
        if(_options.Mode is ListSelectionMode<T>.Multi multiSelectMode)
        {
            await multiSelectMode.OnCommit(new(sp), state.ViewModel.SelectedValues);
            await FinalizeStep(sp);
            return FlowStepResult.Next;
        }
        
        throw new Exception("There should be no finish buttons!");
    }


    public ListSelectionFlowStep(ListSelectionStepOptions<T> options, InteractiveStepBaseOptions optionsBase) : base(optionsBase)
    {
        _options = options;
    }
}