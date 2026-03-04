using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection.DataValidator;
using TeleFlow.Core.Transport.Callbacks;
using static TeleFlow.Core.Commands.Stateful.Steps.DateSelection.DateSelectionMode;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection;

public class DateSelectionStep : CallbackStepBase<DateSelectionStepViewModel>
{
    private readonly DateSelectionStepOptions _options;

    private readonly DateSelectionStepVMConstraints _constraints;


    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, CallbackAction action)
    {
        return state.ViewModel.Page switch
        {
            DateSelectionStepPage.YearSelection  => await HandleYearPageAction(sp, state, action),
            DateSelectionStepPage.MonthSelection => await HandleMonthPageAction(sp, state, action),
            DateSelectionStepPage.DaySelection  => await HandleDayPageAction(sp, state, action),
            _ => throw new InvalidOperationException($"Unexpected DateSelection CommandStepPage value: {state.ViewModel.Page}. The step is in an invalid state.")
        };
    }


    #region YearPage
    
    private async Task<CommandStepResult> HandleYearPageAction(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleYearSelectYear(sp, state, act.Index),
            UiAction.NextPage        => await HandleYearChangePage(sp, state, +1),
            UiAction.PrevPage        => await HandleYearChangePage(sp, state, -1),
            _ => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage)
        };
    }

    private async Task<CommandStepResult> HandleYearSelectYear(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int index)
    {        
        state = state with { ViewModel = state.ViewModel with { YearSelected = index }};

        if(!DateSelectionViewModelValidator.IsValid(state.ViewModel, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage);

        state = state with { ViewModel = state.ViewModel with { Page = DateSelectionStepPage.MonthSelection }};

        await UpsertAndRerender(sp, state);
        

        if(_options.Mode is YearOnly mode)
        {
            await mode.OnCommit(new(sp), index);
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }
        
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleYearChangePage(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int addPageValue)
    {
        if (addPageValue is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(addPageValue), "addPageValue must be +1 or -1.");

        var newVm = state.ViewModel with
        {
            YearPageIndex = state.ViewModel.YearPageIndex + addPageValue
        };

        if (!DateSelectionViewModelValidator.IsValid(newVm, _constraints))
        {
            var message = addPageValue > 0 ? _options.LastPageMessage : _options.FirstPageMessage;
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, message);
        }

        var newState = state with { ViewModel = newVm };

        await UpsertAndRerender(sp, newState);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    #endregion

    #region MonthPage
    
    private async Task<CommandStepResult> HandleMonthPageAction(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, CallbackAction token)
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

    private async Task<CommandStepResult> HandleMonthSelectMonth(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int index)
    {
        var vmAfterMonth = state.ViewModel with { MonthSelected = index };

        if (!DateSelectionViewModelValidator.IsValid(vmAfterMonth, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidMonthMessage);

        DateSelectionStepViewModel nextVm;

        if (_options.Mode is YearMonth mode)
        {
            nextVm = vmAfterMonth with { DateSelectionCompleted = true };

            await UpsertAndRerender(sp, state with { ViewModel = nextVm });

            var year = nextVm.YearSelected;
            await mode.OnCommit(new(sp), (year, index));
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }

        nextVm = vmAfterMonth with { Page = DateSelectionStepPage.DaySelection };

        await UpsertAndRerender(sp, state with { ViewModel = nextVm });
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthChangeYear(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int addYearValue)
    {
        if (addYearValue is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(addYearValue), "addPageValue must be +1 or -1.");

        state = state with
        {
            ViewModel = state.ViewModel with
            {
                YearSelected = state.ViewModel.YearSelected + addYearValue
            }
        };

        if(!DateSelectionViewModelValidator.IsValid(state.ViewModel, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage);
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthGoToPage(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(page, (int)DateSelectionStepPage.YearSelection, nameof(page));
  
        state = state with
        {
            ViewModel = state.ViewModel with
            {
                Page = DateSelectionStepPage.YearSelection
            }
        };
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    #endregion

    #region DayPage

    private async Task<CommandStepResult> HandleDayPageAction(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, CallbackAction token)
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

    private async Task<CommandStepResult> HandleDaySelectDay(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int index)
    {
        if (_options.Mode is not YearMonthDay mode)
            throw new InvalidOperationException($"Date selection page requires '{nameof(YearMonthDay)}' mode, but current mode is '{_options.Mode?.GetType().Name ?? "null"}'.");

        state = state with { ViewModel = state.ViewModel with { DaySelected = index }};

        if(!DateSelectionViewModelValidator.IsValid(state.ViewModel, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidDayMessage);

        state = state with { ViewModel = state.ViewModel with { DateSelectionCompleted = true }};

        await UpsertAndRerender(sp, state);

        var year = state.ViewModel.YearSelected;
        var month = state.ViewModel.MonthSelected;
        var day = state.ViewModel.DaySelected;

        var date = new DateOnly(year, month, day);

        await mode.OnCommit(new(sp), date);
        

        await FinalizeStep(sp);
        return CommandStepResult.Next;
    }

    private async Task<CommandStepResult> HandleDayChangeMonth(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int addMonthValue)
    {
        if (addMonthValue is not (1 or -1))
            throw new ArgumentOutOfRangeException(nameof(addMonthValue), "addMonthValue must be +1 or -1.");

        var vm = state.ViewModel;

        int newYear = vm.YearSelected;
        int newMonth = vm.MonthSelected + addMonthValue;

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
            ViewModel = vm with
            {
                YearSelected = newYear,
                MonthSelected = newMonth
            }
        };

        if (!DateSelectionViewModelValidator.IsValid(state.ViewModel, _constraints))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidYearMessage);

        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleDayGoToPage(IServiceProvider sp, StepState<DateSelectionStepViewModel> state, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(page, (int)DateSelectionStepPage.MonthSelection, nameof(page));

        state = state with
        {
            ViewModel = state.ViewModel with
            {
                Page = DateSelectionStepPage.MonthSelection
            }
        };
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    
    #endregion


    protected override Task<DateSelectionStepViewModel> CreateDefaultViewModel(IServiceProvider sp) 
        => Task.FromResult(DateSelectionStepViewModel.CreateDefault());


    public DateSelectionStep(DateSelectionStepOptions options, DateSelectionStepVMConstraints constraints) : base(options.BaseOptions)
    {
        _options = options;
        _constraints = constraints;
    }
}
