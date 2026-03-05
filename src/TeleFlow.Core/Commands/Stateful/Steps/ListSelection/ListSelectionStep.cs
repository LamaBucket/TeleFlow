using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.DataValidator;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Core.Commands.Stateful.Steps.ListSelection.ListSelectionMode;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction.UiAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.ListSelection;

public class ListSelectionStep<T> : CallbackStep<ListSelectionStepData<T>>
{
    private readonly ListSelectionStepOptions<T> _options;
    
    private readonly ListSelectionStepDataConstraints _constraints;

    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<ListSelectionStepData<T>> state, CallbackAction action)
    {
        return _options.Mode switch
        {
            SingleSelect<T> singleSelect => await HandleSingleSelectAction(sp, singleSelect, state, action),
            MultiSelect<T> multiSelect   => await HandleMultiSelectAction(sp, multiSelect, state, action),
            _ => throw new NotSupportedException($"Unsupported list selection mode: {_options.Mode.GetType()}. Expected {typeof(SingleSelect<T>).FullName} or {typeof(MultiSelect<T>).FullName}.")
        };
    }


    #region  SingleSelect
    
    private async Task<CommandStepResult> HandleSingleSelectAction(IServiceProvider sp, SingleSelect<T> mode, StepState<ListSelectionStepData<T>> state, CallbackAction action)
    {
        return action switch
        {
            SelectIndex act => await HandleSingleSelectSelect(sp, mode, state, act.Index),
            PrevPage        => await HandlePageMoveAsync(sp, state, -1),
            NextPage        => await HandlePageMoveAsync(sp, state, +1),
            _               => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.IndexOutOfRangeMessage)
        };
    }

    private async Task<CommandStepResult> HandleSingleSelectSelect(IServiceProvider sp, SingleSelect<T> mode, StepState<ListSelectionStepData<T>> state, int index)
    {
        var stepData = state.StepData.Toggle(index);
        stepData = stepData with { ListSelectionFinished = true };

        if(!ListSelectionStepDataValidator.IsValid(stepData, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.IndexOutOfRangeMessage);
        
        state = state with { StepData = stepData };
        await UpsertAndRerender(sp, state);

        var item = state.StepData.SelectedValue;
        await mode.OnCommit(new(sp), item);

        await FinalizeStep(sp);
        return CommandStepResult.Next;
    }

    #endregion

    #region  MultiSelect

    private async Task<CommandStepResult> HandleMultiSelectAction(IServiceProvider sp, MultiSelect<T> mode, StepState<ListSelectionStepData<T>> state, CallbackAction action)
    {
        return action switch
        {
            SelectIndex act => await HandleMultiSelectSelect(sp, state, act.Index),
            PrevPage        => await HandlePageMoveAsync(sp, state, -1),
            NextPage        => await HandlePageMoveAsync(sp, state, +1),
            Finish          => await HandleMultiSelectFinish(sp, mode, state),
            _               => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.IndexOutOfRangeMessage)
        };
    }

    private async Task<CommandStepResult> HandleMultiSelectSelect(IServiceProvider sp, StepState<ListSelectionStepData<T>> state, int index)
    {
        var stepData = state.StepData.Toggle(index);
        
        if(!ListSelectionStepDataValidator.IsValid(stepData, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.IndexOutOfRangeMessage);
        
        state = state with { StepData = stepData };
        await UpsertAndRerender(sp, state);
        
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMultiSelectFinish(IServiceProvider sp, MultiSelect<T> mode, StepState<ListSelectionStepData<T>> state)
    {
        state = state with { StepData = state.StepData with { ListSelectionFinished = true } };
        await UpsertAndRerender(sp, state);

        await mode.OnCommit(new(sp), state.StepData.SelectedValues);
        
        await FinalizeStep(sp);
        return CommandStepResult.Next;
    }
    
    #endregion
    
    private async Task<CommandStepResult> HandlePageMoveAsync(IServiceProvider sp, StepState<ListSelectionStepData<T>> state, int addPageValue)
    {
        if (addPageValue is not (1 or -1))
                throw new ArgumentOutOfRangeException(nameof(addPageValue), "addPageValue must be +1 or -1.");
        
        var stateData = state.StepData with { Page = state.StepData.Page + addPageValue };
        
        if(!ListSelectionStepDataValidator.IsValid(stateData, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, addPageValue == 1 ? _options.LastPageMessage : _options.FirstPageMessage);
        
        state = state with { StepData = stateData };

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }


    protected override async Task<ListSelectionStepData<T>> CreateDefaultStepData(IServiceProvider sp)
        => ListSelectionStepData<T>.CreateDefaultData(await _options.ValueProvider.Invoke(sp));

    public ListSelectionStep(ListSelectionStepOptions<T> options, ListSelectionStepDataConstraints constraints) : base(options.BaseOptions)
    {
        _options = options;
        _constraints = constraints;
    }
}