using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public class ListSelectionCommandStep<T> : CallbackCommandStepBase<ListSelectionCommandStepViewModel<T>>
{
    private readonly ListSelectionCommandStepOptions<T> _options;
    
    protected override async Task<ListSelectionCommandStepViewModel<T>> CreateDefaultViewModel(IServiceProvider sp)
    {
        var values = await _options.ValueProvider.Invoke(sp);
        return new(values);
    }

    protected override InlineKeyboardMarkup RenderMarkup(ICallbackCodec markupEncoder, ListSelectionCommandStepViewModel<T> vm)
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

    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state, CallbackAction action)
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
                return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, "Unsupported action.");
        }
    }

    private async Task<CommandStepResult> HandleToggleAsync(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state, int index)
    {
        var vm = state.ViewModel;

        if (index < 0 || index >= vm.Values.Count)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, "Invalid item.");

        if (_options.Mode is ListSelectionMode<T>.Single singleSelectMode)
        {
            await singleSelectMode.OnCommit(new(sp), vm.Values[index]);
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }
        else if (_options.Mode is ListSelectionMode<T>.Multi)
        {
            vm.Toggle(index);
            await UpsertAndRerender(sp, state);
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
        }

        throw new Exception("Mode Unknown!");
    }

    private async Task<CommandStepResult> HandleNextPageAsync(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state)
    {
        var vm = state.ViewModel;

        int pageSize = _options.MaxItemsInRow * _options.MaxRowsInPage;
        int maxPage = vm.Values.Count == 0 ? 0 : (vm.Values.Count - 1) / pageSize;

        if (vm.Page < maxPage) vm.Page++;

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandlePrevPageAsync(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state)
    {
        var vm = state.ViewModel;

        if (vm.Page > 0) vm.Page--;

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleFinishAsync(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state)
    {
        if(_options.Mode is ListSelectionMode<T>.Multi multiSelectMode)
        {
            await multiSelectMode.OnCommit(new(sp), state.ViewModel.SelectedValues);
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }
        
        throw new Exception("There should be no finish buttons!");
    }


    public ListSelectionCommandStep(ListSelectionCommandStepOptions<T> options, CallbackCommandStepBaseOptions optionsBase) : base(optionsBase)
    {
        _options = options;
    }
}