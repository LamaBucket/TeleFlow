using TeleFlow.Models.CommandResults;

namespace TeleFlow.Models.MultiStep;

public class StepResult
{
    public StepAction Action { get; init; }

    public int GoToStepNumber { get; init; } = -1;

    public StepHoldOnReason HoldOnReason { get; init; } = StepHoldOnReason.None;

    public string? HoldOnMessage { get; init; } = null;


    public static StepResult Next => new(StepAction.MoveNext);

    public static StepResult Previous => new(StepAction.MovePrevious);

    public static StepResult HoldOn(StepHoldOnReason reason, string? holdOnMessage = null) => new(reason, holdOnMessage);

    public static StepResult GoTo(int stepNumber) => new(stepNumber);


    private StepResult(StepAction action)
    {
        if(action == StepAction.GoTo)
            throw new Exception("Use another constructor to go to a specific step");
        else if(action == StepAction.HoldOn)
            throw new Exception("Use another constructor to hold on a step");
        
        Action = action;
    }

    private StepResult(int goToStepNumber)
    {
        if(goToStepNumber < 0)
            throw new Exception("");

        Action = StepAction.GoTo;
        GoToStepNumber = goToStepNumber;
    }

    private StepResult(StepHoldOnReason reason, string? holdOnMessage)
    {
        if(reason == StepHoldOnReason.None)
            throw new Exception("");

        Action = StepAction.HoldOn;
        HoldOnReason = reason;
        HoldOnMessage = holdOnMessage;
    }
}


public enum StepAction
{
    MoveNext,
    HoldOn,
    MovePrevious,
    GoTo
}

public enum StepHoldOnReason
{
    None,
    InvalidInput,
    Other
}