using Microsoft.AspNetCore.Http.HttpResults;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection;
using TeleFlow.Core.Transport.Callbacks;

namespace TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput;

public class ConfirmInputStep : CallbackStep<ConfirmInputStepData>
{
    private readonly ConfirmInputStepOptions _options;
    
    protected override async Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<ConfirmInputStepData> state, CallbackAction action)
    {
        if (action is not CallbackAction.UiAction.SelectIndex indexAction)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidButtonMessage);
        
        if(indexAction.Index != 0 && indexAction.Index != 1)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidButtonMessage);

        bool value = indexAction.Index == 1;

        var data = new ConfirmInputStepData(value);
        
        state = state with { StepData = data };
        await UpsertAndRerender(sp, state);
        await FinalizeStep(sp);

        return await _options.OnCommit(new(sp), value);
    }
    
    protected override Task<ConfirmInputStepData> CreateDefaultStepData(IServiceProvider sp)
        => Task.FromResult(ConfirmInputStepData.CreateDefault());
    
    public ConfirmInputStep(ConfirmInputStepOptions options) : base(options.CallbackStepOptions)
    {
        _options = options;
    }
}