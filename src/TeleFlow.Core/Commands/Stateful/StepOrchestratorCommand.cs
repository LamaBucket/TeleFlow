using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Commands.Results.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using static TeleFlow.Abstractions.State.Chat.ChatSession;

namespace TeleFlow.Core.Commands.Stateful;

public class StepOrchestratorCommand : ICommandHandler
{
    private readonly ChatSessionStepSnapshot _stepState;

    private readonly CommandStepRouter _stepCommandFactory;

    private readonly Func<IServiceProvider, Task<CommandResult>> _onCompleted; 


    protected virtual bool InitializeNextStep => true;


    public async Task<CommandResult> Handle(UpdateContext update)
    {
        var stepCommand = _stepCommandFactory.Create(_stepState.CurrentCommandStep);

        if (!_stepState.IsStepInitialized)
        {
            await stepCommand.OnEnter(update.ServiceProvider);

            return HoldOnStatefulResult.Initialize;
        }

        var result = await stepCommand.Handle(update);

        if(result.Action == CommandStepAction.HoldOn)
        {
            var holdOnReason = GetMiddlewareReasonFromStepReason(result.HoldOnReason);

            return new HoldOnStatefulResult(holdOnReason, result.HoldOnMessage);
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
        
        return new GoToStatefulResult(nextStepNum, InitializeNextStep);
    }

    private int CalculateNextStepNum(CommandStepResult result)
    {
        return result.Action switch
        {
            CommandStepAction.MoveNext => _stepState.CurrentCommandStep + 1,
            CommandStepAction.MovePrevious => _stepState.CurrentCommandStep - 1,
            CommandStepAction.GoTo => result.GoToStepNumber,
            _ => -1
        };
    }

    private HoldOnReason GetMiddlewareReasonFromStepReason(CommandStepHoldOnReason reason)
    {
        return reason switch
        {
            CommandStepHoldOnReason.InvalidInput => HoldOnReason.InvalidInput,
            CommandStepHoldOnReason.Other => HoldOnReason.Other,
            _ => throw new Exception("Reason not supported")
        };
    }

    public StepOrchestratorCommand(ChatSessionStepSnapshot stepState, CommandStepRouter stepCommandFactory, Func<IServiceProvider, Task<CommandResult>> onCompleted)
    {
        _stepState = stepState;
        _stepCommandFactory = stepCommandFactory;
        _onCompleted = onCompleted;
    }
}