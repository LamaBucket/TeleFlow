using TeleFlow.Commands.Results;

namespace TeleFlow.Commands.Results.MultiStep;

public class HoldOnMultiStepResult : CommandResult
{
    public HoldOnReason Reason { get; init; }

    public string? HoldOnMessage { get; init; }


    public static HoldOnMultiStepResult Initialize => new();

    public HoldOnMultiStepResult(HoldOnReason reason, string? holdOnMessage)
    {
        if(reason == HoldOnReason.Initialize)
            throw new Exception("Use the Initialize static property to create an initialize hold on result.");

        Reason = reason;
        HoldOnMessage = holdOnMessage;
    }

    private HoldOnMultiStepResult()
    {
        Reason = HoldOnReason.Initialize;
    }
}

public enum HoldOnReason
{
    Initialize,
    InvalidInput,
    Other
}