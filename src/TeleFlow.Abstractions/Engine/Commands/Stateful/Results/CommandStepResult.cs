namespace TeleFlow.Abstractions.Engine.Commands.Stateful.Results;

public class CommandStepResult
{
    public CommandStepAction Action { get; init; }

    public int GoToStepNumber { get; init; } = -1;

    public CommandStepHoldOnReason HoldOnReason { get; init; } = CommandStepHoldOnReason.None;

    public string? HoldOnMessage { get; init; } = null;


    public static CommandStepResult Next => new(CommandStepAction.MoveNext);

    public static CommandStepResult Previous => new(CommandStepAction.MovePrevious);

    public static CommandStepResult HoldOn(CommandStepHoldOnReason reason, string? holdOnMessage = null) => new(reason, holdOnMessage);

    public static CommandStepResult GoTo(int stepNumber) => new(stepNumber);


    private CommandStepResult(CommandStepAction action)
    {
        if(action == CommandStepAction.GoTo)
            throw new ArgumentException("Use CommandStepResult.GoTo(stepNumber) to create a GoTo result.", nameof(action));
        else if(action == CommandStepAction.HoldOn)
            throw new ArgumentException("Use CommandStepResult.HoldOn(reason, message) to create a HoldOn result.", nameof(action));
        
        Action = action;
    }

    private CommandStepResult(int goToStepNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(goToStepNumber, nameof(goToStepNumber));

        Action = CommandStepAction.GoTo;
        GoToStepNumber = goToStepNumber;
    }

    private CommandStepResult(CommandStepHoldOnReason reason, string? holdOnMessage)
    {
        if(reason == CommandStepHoldOnReason.None)
            throw new ArgumentException("HoldOn requires a specific reason. Use CommandStepHoldOnReason.InvalidInput or CommandStepHoldOnReason.Other.", nameof(reason));

        Action = CommandStepAction.HoldOn;
        HoldOnReason = reason;
        HoldOnMessage = holdOnMessage;
    }
}


public enum CommandStepAction
{
    MoveNext,
    HoldOn,
    MovePrevious,
    GoTo
}

public enum CommandStepHoldOnReason
{
    None,
    InvalidInput,
    Other
}