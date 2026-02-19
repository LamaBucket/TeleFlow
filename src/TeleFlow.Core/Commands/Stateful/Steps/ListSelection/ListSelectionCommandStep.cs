using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.Step;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.Ui;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public class ListSelectionCommandStep<T> : CallbackCommandStepBase<ListSelectionCommandStepViewModel<T>>
{
    private readonly ListSelectionCommandStepOptions<T> _options;
    
    protected override async Task<ListSelectionCommandStepViewModel<T>> CreateDefaultViewModel(IServiceProvider sp)
        => new(await _options.ValueProvider.Invoke(sp));

    protected override InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec, ListSelectionCommandStepViewModel<T> vm)
        => ListSelectionRenderService.RenderMarkup<T>(_options, markupButtonActionCodec, vm);


    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state, CallbackAction action)
    {
        return action switch
        {
            SelectIndex toggle => await HandleToggleAsync(sp, state, toggle.Index),
            Finish => await HandleFinishAsync(sp, state),
            NextPage => await HandleNextPageAsync(sp, state),
            PrevPage => await HandlePrevPageAsync(sp, state),
            _ => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, "Unsupported action."),
        };
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


    public ListSelectionCommandStep(ListSelectionCommandStepOptions<T> options, CallbackCommandStepBaseOptions optionsBase) : base(optionsBase)
    {
        _options = options;
    }
}