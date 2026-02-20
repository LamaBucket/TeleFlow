using System.Collections;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;
using TeleFlow.Core.Commands.Stateful.Steps.DateInput.Internal;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types.ReplyMarkups;
using static TeleFlow.Core.Commands.Stateful.Steps.DateInput.DateInputMode;
using static TeleFlow.Core.Transport.Callbacks.CallbackAction;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateInput;

public class DateInputCommandStep : CallbackCommandStepBase<DateInputCommandStepViewModel>
{
    private readonly DateInputCommandStepOptions _options;


    protected override async Task<DateInputCommandStepViewModel> CreateDefaultViewModel(IServiceProvider sp) 
        => new(); 

    protected override InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec, DateInputCommandStepViewModel vm)
        => DateInputRenderService.RenderMarkup(_options, markupButtonActionCodec, vm);


    protected override Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, CallbackAction token)
    {
        return state.ViewModel.Page switch
        {
            DateInputCommandStepPage.YearSelection  => HandleYearPageAction(sp, state, token),
            DateInputCommandStepPage.MonthSelection => HandleMonthPageAction(sp, state, token),
            DateInputCommandStepPage.DateSelection  => HandleDatePageAction(sp, state, token),
            _ => throw new Exception("Unknown page state")
        };
    }


    #region YearPage
    
    private async Task<CommandStepResult> HandleYearPageAction(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleYearSelectIndex(sp, state, act.Index),
            UiAction.NextPage        => await HandleYearNextPage(sp, state),
            UiAction.PrevPage        => await HandleYearPrevPage(sp, state),
            UiAction.NoOperation     => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, "Please select a year"),
            _ => throw new Exception("ploho vse")
        };
    }

    private async Task<CommandStepResult> HandleYearSelectIndex(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, int index)
    {        
        if(_options.Mode is YearOnly mode)
        {
            await mode.OnCommit(new(sp), index);
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }

        state.ViewModel.YearSelected = index;
        state.ViewModel.Page = DateInputCommandStepPage.MonthSelection;

        await UpsertAndRerender(sp, state);
        
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleYearNextPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state)
    {
        state.ViewModel.YearPageIndex += 1;
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleYearPrevPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state)
    {
        state.ViewModel.YearPageIndex -= 1;
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    #endregion

    #region MonthPage
    
    private async Task<CommandStepResult> HandleMonthPageAction(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleMonthSelectIndex(sp, state, act.Index),
            UiAction.NextPage        => await HandleMonthNextPage(sp, state),
            UiAction.PrevPage        => await HandleMonthPrevPage(sp, state),
            UiAction.GoToPage goTo   => await HandleMonthGoToPage(sp, state, goTo.Page),
            UiAction.NoOperation     => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, "Please select a month"),
            _ => throw new Exception("ploho vse")
        };
    }

    private async Task<CommandStepResult> HandleMonthSelectIndex(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, int index)
    {
        if(_options.Mode is YearMonth mode)
        {
            var year = state.ViewModel.YearSelected;

            await mode.OnCommit(new(sp), (year, index));
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }

        state.ViewModel.MonthSelected = index;
        state.ViewModel.Page = DateInputCommandStepPage.DateSelection;

        await UpsertAndRerender(sp, state);
        
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthNextPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state)
    {
        state.ViewModel.YearSelected += 1;
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthPrevPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state)
    {
        state.ViewModel.YearSelected -= 1;
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleMonthGoToPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, int page)
    {
        if(page != 0)
            throw new Exception("Invalid Page State");

        state.ViewModel.Page = DateInputCommandStepPage.YearSelection;
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    #endregion

    #region DayPage

    private async Task<CommandStepResult> HandleDatePageAction(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, CallbackAction token)
    {
        return token switch
        {
            UiAction.SelectIndex act => await HandleDaySelectIndex(sp, state, act.Index),
            UiAction.NextPage        => await HandleDayNextPage(sp, state),
            UiAction.PrevPage        => await HandleDayPrevPage(sp, state),
            UiAction.GoToPage goTo   => await HandleDayGoToPage(sp, state, goTo.Page),
            UiAction.NoOperation     => CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, "Please select a month"),
            _ => throw new Exception("ploho vse")
        };
    }

    private async Task<CommandStepResult> HandleDaySelectIndex(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, int index)
    {
        state.ViewModel.DaySelected = index;

        if(_options.Mode is YearMonthDay mode)
        {
            var year = state.ViewModel.YearSelected;
            var month = state.ViewModel.MonthSelected;
            var day = state.ViewModel.DaySelected;

            var date = new DateOnly(year, month, day);

            await mode.OnCommit(new(sp), date);
            await FinalizeStep(sp);
            return CommandStepResult.Next;
        }

        throw new Exception("Invalid state");
    }

    private async Task<CommandStepResult> HandleDayNextPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state)
    {
        if(state.ViewModel.MonthSelected < 12)
        {
            state.ViewModel.MonthSelected += 1;
        }
        else
        {
            state.ViewModel.MonthSelected = 1;
            state.ViewModel.YearSelected += 1;
        }
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleDayPrevPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state)
    {
        if(state.ViewModel.MonthSelected > 1)
        {
            state.ViewModel.MonthSelected -= 1;
        }
        else
        {
            state.ViewModel.MonthSelected = 12;
            state.ViewModel.YearSelected -= 1;
        }
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    private async Task<CommandStepResult> HandleDayGoToPage(IServiceProvider sp, StepState<DateInputCommandStepViewModel> state, int page)
    {
        if(page != 1)
            throw new Exception("Invalid Page State");

        state.ViewModel.Page = DateInputCommandStepPage.MonthSelection;
        
        await UpsertAndRerender(sp, state);
        return CommandStepResult.HoldOn(CommandStepHoldOnReason.Other);
    }

    
    #endregion


    public DateInputCommandStep(DateInputCommandStepOptions options, CallbackCommandStepBaseOptions optionsBase) : base(optionsBase)
    {
        _options = options;
    }
}
