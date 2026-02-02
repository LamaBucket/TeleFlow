namespace TeleFlow.Models.CommandResults;

public class HoldOnMultiStepResult : CommandResult
{
    public HoldOnReason Reason { get; init; }

    public string? HoldOnMessage { get; init; }


    public static HoldOnMultiStepResult Initialize => new HoldOnMultiStepResult(HoldOnReason.Initialize, null);

    public HoldOnMultiStepResult(HoldOnReason reason, string? holdOnMessage)
    {
        if(reason == HoldOnReason.Initialize)
            throw new Exception("Use the Initialize static property to create an initialize hold on result.");

        Reason = reason;
        HoldOnMessage = holdOnMessage;
    }
}

public enum HoldOnReason
{
    Initialize,
    InvalidInput,
    Other
}