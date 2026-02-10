using System.Diagnostics;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;

namespace TeleFlow.Commands.Statefull;

public class FlowCommand : ICommandHandler
{
    private readonly ChatSessionStepSnapshot _stepState;

    private readonly FlowStepResolver _stepCommandFactory;

    private readonly Func<IServiceProvider, Task<CommandResult>> _onCompleted; 


    protected virtual bool InitializeNextStep => true;


    public async Task<CommandResult> Handle(UpdateContext update)
    {
        var stepCommand = _stepCommandFactory.Create(_stepState.CurrentCommandStep);

        if (!_stepState.IsStepInitialized)
        {
            await stepCommand.OnEnter(update.ServiceProvider);

            return HoldOnMultiStepResult.Initialize;
        }

        var result = await stepCommand.Handle(update);

        if(result.Action == FlowStepAction.HoldOn)
        {
            var holdOnReason = GetMiddlewareReasonFromStepReason(result.HoldOnReason);

            return new HoldOnMultiStepResult(holdOnReason, result.HoldOnMessage);
        }   

        var nextStepNum = CalculateNextStepNum(result);

        if(nextStepNum < 0)
            throw new Exception("Step Number Cannot be less than 0!");

        if(nextStepNum >= _stepCommandFactory.StepCount)
            return await _onCompleted.Invoke(update.ServiceProvider);

        if (InitializeNextStep)
        {
            var next = _stepCommandFactory.Create(nextStepNum);

            await next.OnEnter(update.ServiceProvider);
        }
        
        return new GoToMultiStepResult(nextStepNum, InitializeNextStep);
    }

    private int CalculateNextStepNum(FlowStepResult result)
    {
        return result.Action switch
        {
            FlowStepAction.MoveNext => _stepState.CurrentCommandStep + 1,
            FlowStepAction.MovePrevious => _stepState.CurrentCommandStep - 1,
            FlowStepAction.GoTo => result.GoToStepNumber,
            _ => -1
        };
    }

    private HoldOnReason GetMiddlewareReasonFromStepReason(FlowStepHoldOnReason reason)
    {
        return reason switch
        {
            FlowStepHoldOnReason.InvalidInput => HoldOnReason.InvalidInput,
            FlowStepHoldOnReason.Other => HoldOnReason.Other,
            _ => throw new Exception("Reason not supported")
        };
    }

    public FlowCommand(ChatSessionStepSnapshot stepState, FlowStepResolver stepCommandFactory, Func<IServiceProvider, Task<CommandResult>> onCompleted)
    {
        _stepState = stepState;
        _stepCommandFactory = stepCommandFactory;
        _onCompleted = onCompleted;
    }
}