namespace TeleFlow.Commands.Flow;

public class FlowStepResult
{
    public FlowStepAction Action { get; init; }

    public int GoToStepNumber { get; init; } = -1;

    public FlowStepHoldOnReason HoldOnReason { get; init; } = FlowStepHoldOnReason.None;

    public string? HoldOnMessage { get; init; } = null;


    public static FlowStepResult Next => new(FlowStepAction.MoveNext);

    public static FlowStepResult Previous => new(FlowStepAction.MovePrevious);

    public static FlowStepResult HoldOn(FlowStepHoldOnReason reason, string? holdOnMessage = null) => new(reason, holdOnMessage);

    public static FlowStepResult GoTo(int stepNumber) => new(stepNumber);


    private FlowStepResult(FlowStepAction action)
    {
        if(action == FlowStepAction.GoTo)
            throw new Exception("Use another constructor to go to a specific step");
        else if(action == FlowStepAction.HoldOn)
            throw new Exception("Use another constructor to hold on a step");
        
        Action = action;
    }

    private FlowStepResult(int goToStepNumber)
    {
        if(goToStepNumber < 0)
            throw new Exception("");

        Action = FlowStepAction.GoTo;
        GoToStepNumber = goToStepNumber;
    }

    private FlowStepResult(FlowStepHoldOnReason reason, string? holdOnMessage)
    {
        if(reason == FlowStepHoldOnReason.None)
            throw new Exception("");

        Action = FlowStepAction.HoldOn;
        HoldOnReason = reason;
        HoldOnMessage = holdOnMessage;
    }
}


public enum FlowStepAction
{
    MoveNext,
    HoldOn,
    MovePrevious,
    GoTo
}

public enum FlowStepHoldOnReason
{
    None,
    InvalidInput,
    Other
}