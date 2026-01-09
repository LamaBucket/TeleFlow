namespace TeleFlow.Models.CommandResults;

public class HoldOnMultiStepResult : CommandResult
{
    public HoldOnReason Reason { get; init; }

    public HoldOnMultiStepResult(HoldOnReason reason)
    {
        Reason = reason;
    }
}

public enum HoldOnReason
{
    Initialize,
    InvalidInput,
    Other
}