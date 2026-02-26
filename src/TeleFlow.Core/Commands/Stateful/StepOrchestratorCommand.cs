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

    private readonly CommandStepRouter _stepRouter;

    private readonly Func<IServiceProvider, Task<CommandResult>> _onCompleted; 


    protected virtual bool InitializeNextStep => true;


    public async Task<CommandResult> Handle(UpdateContext update)
    {
        var stepCommand = _stepRouter.Create(_stepState.CurrentCommandStep);

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
            throw new InvalidOperationException($"Invalid step transition. Current step: {_stepState.CurrentCommandStep}, action: {result.Action}.");

        if(nextStepNum >= _stepRouter.StepCount)
            return await _onCompleted.Invoke(update.ServiceProvider);

        if (InitializeNextStep)
        {
            var next = _stepRouter.Create(nextStepNum);

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
            _ => throw new NotSupportedException($"Unsupported {nameof(CommandStepHoldOnReason)} value: {reason}.")
        };
    }

    public StepOrchestratorCommand(ChatSessionStepSnapshot stepState, CommandStepRouter stepRouter, Func<IServiceProvider, Task<CommandResult>> onCompleted)
    {
        _stepState = stepState;
        _stepRouter = stepRouter;
        _onCompleted = onCompleted;
    }
}