using TeleFlow.Abstractions.Engine.Commands.Results.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Internal;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration.ListSelectionMode;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.UiAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public class ListSelectionCommandStep<T> : CallbackCommandStepBase<ListSelectionCommandStepViewModel<T>>
{
    private readonly ListSelectionOptions<T> _options;
    
    protected override async Task<ListSelectionCommandStepViewModel<T>> CreateDefaultViewModel(IServiceProvider sp)
        => new(await _options.ValueProvider.Invoke(sp));

    protected override InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec, ListSelectionCommandStepViewModel<T> vm)
        => ListSelectionRenderService.RenderMarkup<T>(_options, markupButtonActionCodec, vm);


    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state, CallbackAction action)
    {
        return _options.Mode switch
        {
            SingleSelect<T> singleSelect => await HandleSingleSelectAction(sp, singleSelect, state, action),
            MultiSelect<T> multiSelect   => await HandleMultiSelectAction(sp, multiSelect, state, action),
            _ => throw new NotSupportedException($"Unsupported list selection mode: {_options.Mode.GetType()}. Expected {typeof(SingleSelect<T>).FullName} or {typeof(MultiSelect<T>).FullName}.")
        };
    }


    #region  SingleSelect
    
    private async Task<CommandStepResult> HandleSingleSelectAction(IServiceProvider sp, SingleSelect<T> mode, StepState<ListSelectionCommandStepViewModel<T>> state, CallbackAction action)
    {
        return action switch
        {
            SelectIndex act => await HandleSingleSelectSelect(sp, mode, state, act.Index),
            PrevPage        => await HandlePrevPageAsync(sp, state),
            NextPage        => await HandleNextPageAsync(sp, state),
            _               => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.ErrorMessageOptions.IndexOutOfRangeMessage)
        };
    }

    private async Task<CommandStepResult> HandleSingleSelectSelect(IServiceProvider sp, SingleSelect<T> mode, StepState<ListSelectionCommandStepViewModel<T>> state, int index)
    {
        if(!ListSelectionViewModelEditor.Toggle(state.ViewModel, index))
        {
            var errors = _options.ErrorMessageOptions;
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, errors.IndexOutOfRangeMessage);
        }

        var item = state.ViewModel.SelectedValue;
        await mode.OnCommit(new(sp), item);

        await FinalizeStep(sp);
        return CommandStepResult.Next;
    }

    #endregion

    #region  MultiSelect

    private async Task<CommandStepResult> HandleMultiSelectAction(IServiceProvider sp, MultiSelect<T> mode, StepState<ListSelectionCommandStepViewModel<T>> state, CallbackAction action)
    {
        return action switch
        {
            SelectIndex act => await HandleMultiSelectSelect(sp, state, act.Index),
            PrevPage        => await HandlePrevPageAsync(sp, state),
            NextPage        => await HandleNextPageAsync(sp, state),
            Finish          => await HandleMultiSelectFinish(sp, mode, state),
            _               => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.ErrorMessageOptions.IndexOutOfRangeMessage)
        };
    }

    private async Task<CommandStepResult> HandleMultiSelectSelect(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state, int index)
    {
        if(!ListSelectionViewModelEditor.Toggle(state.ViewModel, index))
        {
            var errors = _options.ErrorMessageOptions;
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, errors.IndexOutOfRangeMessage);
        }
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMultiSelectFinish(IServiceProvider sp, MultiSelect<T> mode, StepState<ListSelectionCommandStepViewModel<T>> state)
    {
        await mode.OnCommit(new(sp), state.ViewModel.SelectedValues);
        
        await FinalizeStep(sp);
        return CommandStepResult.Next;
    }
    
    #endregion

    #region Common
    
    private async Task<CommandStepResult> HandleNextPageAsync(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state)
    {
        if (!ListSelectionViewModelEditor.NextPage(state.ViewModel, _options.PageSizeOptions))
        {
            var errors = _options.ErrorMessageOptions;
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, errors.LastPageMessage);
        }

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandlePrevPageAsync(IServiceProvider sp, StepState<ListSelectionCommandStepViewModel<T>> state)
    {
        if (!ListSelectionViewModelEditor.PrevPage(state.ViewModel))
        {
            var errors = _options.ErrorMessageOptions;
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, errors.FirstPageMessage);
        }

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }
    
    #endregion


    public ListSelectionCommandStep(ListSelectionOptions<T> options, CallbackCommandStepBaseOptions optionsBase) : base(optionsBase)
    {
        _options = options;
    }
}