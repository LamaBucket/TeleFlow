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
            throw new Exception("Use another constructor to go to a specific step");
        else if(action == CommandStepAction.HoldOn)
            throw new Exception("Use another constructor to hold on a step");
        
        Action = action;
    }

    private CommandStepResult(int goToStepNumber)
    {
        if(goToStepNumber < 0)
            throw new Exception("");

        Action = CommandStepAction.GoTo;
        GoToStepNumber = goToStepNumber;
    }

    private CommandStepResult(CommandStepHoldOnReason reason, string? holdOnMessage)
    {
        if(reason == CommandStepHoldOnReason.None)
            throw new Exception("");

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