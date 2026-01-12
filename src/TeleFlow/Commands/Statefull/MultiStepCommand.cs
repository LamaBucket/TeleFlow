using System.Diagnostics;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;

namespace TeleFlow.Commands.Statefull;

public class MultiStepCommand : ICommandHandler
{
    private readonly ChatSessionStepState _stepState;

    private readonly StepCommandFactory _stepCommandFactory;

    private readonly Func<IServiceProvider, Task<CommandResult>> _onCompleted; 


    protected virtual bool InitializeNextStep => true;


    public async Task<CommandResult> Handle(UpdateContext update)
    {
        var stepCommand = _stepCommandFactory.Create(_stepState.CurrentCommandStep);

        if (!_stepState.IsStepInitialized)
        {
            await stepCommand.OnEnter(update.ServiceProvider);

            return new HoldOnMultiStepResult(HoldOnReason.Initialize);
        }

        var result = await stepCommand.Handle(update);

        if(result.Action == StepAction.HoldOn)
        {
            var holdOnReason = GetMiddlewareReasonFromStepReason(result.HoldOnReason);

            return new HoldOnMultiStepResult(holdOnReason);
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

    private int CalculateNextStepNum(StepResult result)
    {
        return result.Action switch
        {
            StepAction.MoveNext => _stepState.CurrentCommandStep + 1,
            StepAction.MovePrevious => _stepState.CurrentCommandStep - 1,
            StepAction.GoTo => result.GoToStepNumber,
            _ => -1
        };
    }

    private HoldOnReason GetMiddlewareReasonFromStepReason(StepHoldOnReason reason)
    {
        return reason switch
        {
            StepHoldOnReason.InvalidInput => HoldOnReason.InvalidInput,
            StepHoldOnReason.Other => HoldOnReason.Other,
            _ => throw new Exception("Reason not supported")
        };
    }

    public MultiStepCommand(ChatSessionStepState stepState, StepCommandFactory stepCommandFactory, Func<IServiceProvider, Task<CommandResult>> onCompleted)
    {
        _stepState = stepState;
        _stepCommandFactory = stepCommandFactory;
        _onCompleted = onCompleted;
    }
}