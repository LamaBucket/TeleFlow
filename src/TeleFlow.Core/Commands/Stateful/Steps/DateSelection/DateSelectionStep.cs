using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection.DataValidator;
using TeleFlow.Core.Transport.Callbacks;
using static TeleFlow.Core.Commands.Stateful.Steps.DateSelection.DateSelectionMode;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public class DateSelectionStep : CallbackStep<DateSelectionStepData>
{
    private readonly DateSelectionStepOptions _options;

    private readonly DateSelectionStepDataConstraints _constraints;


    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<DateSelectionStepData> state, CallbackAction action)
    {
        return state.StepData.Page switch
        {
            DateSelectionStepPage.YearSelection  => await HandleYearPageAction(sp, state, action),
            DateSelectionStepPage.MonthSelection => await HandleMonthPageAction(sp, state, action),
            DateSelectionStepPage.DaySelection  => await HandleDayPageAction(sp, state, action),
            _ => throw new InvalidOperationException($"Unexpected DateSelection CommandStepPage value: {state.StepData.Page}. The step is in an invalid state.")
        };
    }


    #region YearPage
    
    private async Task<CommandStepResult> HandleYearPageAction(IServiceProvider sp, StepState<DateSelectionStepData> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleYearSelectYear(sp, state, act.Index),
            UiAction.NextPage        => await HandleYearChangePage(sp, state, +1),
            UiAction.PrevPage        => await HandleYearChangePage(sp, state, -1),
            _ => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage)
        };
    }

    private async Task<CommandStepResult> HandleYearSelectYear(IServiceProvider sp, StepState<DateSelectionStepData> state, int index)
    {     
        var vmAfterYear = state.StepData with { YearSelected = index };

        if (!DateSelectionStepDataValidator.IsValid(vmAfterYear, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage);

        DateSelectionStepData nextVm;

        if (_options.Mode is YearOnly mode)
        {
            nextVm = vmAfterYear with { DateSelectionCompleted = true };

            await UpsertAndRerender(sp, state with { StepData = nextVm });

            var year = nextVm.YearSelected;
            await mode.OnCommit(new(sp), year.Value);
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }   
        
        nextVm = vmAfterYear with { Page = DateSelectionStepPage.MonthSelection };

        await UpsertAndRerender(sp, state with { StepData = nextVm });
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleYearChangePage(IServiceProvider sp, StepState<DateSelectionStepData> state, int addPageValue)
    {
        if (addPageValue is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(addPageValue), "addPageValue must be +1 or -1.");

        var newVm = state.StepData with
        {
            YearPageIndex = state.StepData.YearPageIndex + addPageValue
        };

        if (!DateSelectionStepDataValidator.IsValid(newVm, _constraints))
        {
            var message = addPageValue > 0 ? _options.LastPageMessage : _options.FirstPageMessage;
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, message);
        }

        var newState = state with { StepData = newVm };

        await UpsertAndRerender(sp, newState);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    #endregion

    #region MonthPage
    
    private async Task<CommandStepResult> HandleMonthPageAction(IServiceProvider sp, StepState<DateSelectionStepData> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleMonthSelectMonth(sp, state, act.Index),
            UiAction.NextPage        => await HandleMonthChangeYear(sp, state, +1),
            UiAction.PrevPage        => await HandleMonthChangeYear(sp, state, -1),
            UiAction.GoToPage goTo   => await HandleMonthGoToPage(sp, state, goTo.Page),
            _ => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidMonthMessage)
        };
    }

    private async Task<CommandStepResult> HandleMonthSelectMonth(IServiceProvider sp, StepState<DateSelectionStepData> state, int index)
    {
        var vmAfterMonth = state.StepData with { MonthSelected = index };

        if (!DateSelectionStepDataValidator.IsValid(vmAfterMonth, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidMonthMessage);

        DateSelectionStepData nextVm;

        if (_options.Mode is YearMonth mode)
        {
            nextVm = vmAfterMonth with { DateSelectionCompleted = true };

            await UpsertAndRerender(sp, state with { StepData = nextVm });

            var year = nextVm.YearSelected;
            
            if(!year.HasValue)
                throw new Exception("Year must be specified");
            
            await mode.OnCommit(new(sp), (year.Value, index));
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }

        nextVm = vmAfterMonth with { Page = DateSelectionStepPage.DaySelection };

        await UpsertAndRerender(sp, state with { StepData = nextVm });
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthChangeYear(IServiceProvider sp, StepState<DateSelectionStepData> state, int addYearValue)
    {
        if (addYearValue is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(addYearValue), "addPageValue must be +1 or -1.");

        state = state with
        {
            StepData = state.StepData with
            {
                YearSelected = state.StepData.YearSelected + addYearValue
            }
        };

        if(!DateSelectionStepDataValidator.IsValid(state.StepData, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage);
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthGoToPage(IServiceProvider sp, StepState<DateSelectionStepData> state, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(page, (int)DateSelectionStepPage.YearSelection);
  
        state = state with
        {
            StepData = state.StepData with
            {
                Page = DateSelectionStepPage.YearSelection
            }
        };
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    #endregion

    #region DayPage

    private async Task<CommandStepResult> HandleDayPageAction(IServiceProvider sp, StepState<DateSelectionStepData> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleDaySelectDay(sp, state, act.Index),
            UiAction.NextPage        => await HandleDayChangeMonth(sp, state, +1),
            UiAction.PrevPage        => await HandleDayChangeMonth(sp, state, -1),
            UiAction.GoToPage goTo   => await HandleDayGoToPage(sp, state, goTo.Page),
            _ => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidDayMessage)
        };
    }

    private async Task<CommandStepResult> HandleDaySelectDay(IServiceProvider sp, StepState<DateSelectionStepData> state, int index)
    {
        if (_options.Mode is not YearMonthDay mode)
            throw new InvalidOperationException($"Date selection page requires '{nameof(YearMonthDay)}' mode, but current mode is '{_options.Mode?.GetType().Name ?? "null"}'.");

        state = state with { StepData = state.StepData with { DaySelected = index }};

        if(!DateSelectionStepDataValidator.IsValid(state.StepData, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidDayMessage);

        state = state with { StepData = state.StepData with { DateSelectionCompleted = true }};

        await UpsertAndRerender(sp, state);

        var data = state.StepData;
        
        if(!data.YearSelected.HasValue || !data.MonthSelected.HasValue)
            throw new Exception($"Date selection page requires '{nameof(DateSelectionStepData)}'.");
        
        var year = data.YearSelected.Value;
        var month = data.MonthSelected.Value;
        var day = data.DaySelected.Value;

        var date = new DateOnly(year, month, day);

        await mode.OnCommit(new(sp), date);
        

        await FinalizeStep(sp);
        return CommandStepResult.Next;
    }

    private async Task<CommandStepResult> HandleDayChangeMonth(IServiceProvider sp, StepState<DateSelectionStepData> state, int addMonthValue)
    {
        if (addMonthValue is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(addMonthValue), "addMonthValue must be +1 or -1.");

        var data = state.StepData;
        
        if(!data.YearSelected.HasValue || !data.MonthSelected.HasValue)
            throw new Exception($"Date selection page requires '{nameof(DateSelectionStepData)}'.");
        
        int newYear = data.YearSelected.Value;
        int newMonth = data.MonthSelected.Value + addMonthValue;

        if (newMonth == 13)
        {
            newMonth = 1;
            newYear += 1;
        }
        else if (newMonth == 0)
        {
            newMonth = 12;
            newYear -= 1;
        }

        state = state with
        {
            StepData = data with
            {
                YearSelected = newYear,
                MonthSelected = newMonth
            }
        };

        if (!DateSelectionStepDataValidator.IsValid(state.StepData, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage);

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleDayGoToPage(IServiceProvider sp, StepState<DateSelectionStepData> state, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(page, (int)DateSelectionStepPage.MonthSelection);

        state = state with
        {
            StepData = state.StepData with
            {
                Page = DateSelectionStepPage.MonthSelection
            }
        };
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    
    #endregion


    protected override Task<DateSelectionStepData> CreateDefaultStepData(IServiceProvider sp) 
        => Task.FromResult(DateSelectionStepData.CreateDefault());


    public DateSelectionStep(DateSelectionStepOptions options, DateSelectionStepDataConstraints constraints) : base(options.BaseOptions)
    {
        _options = options;
        _constraints = constraints;
    }
}
