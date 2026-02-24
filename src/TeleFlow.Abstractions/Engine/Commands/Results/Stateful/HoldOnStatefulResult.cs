namespace TeleFlow.Abstractions.Engine.Commands.Results.Stateful;

public class HoldOnStatefulResult : CommandResult
{
    public HoldOnReason Reason { get; init; }

    public string? HoldOnMessage { get; init; }


    public static HoldOnStatefulResult Initialize => new();

    public HoldOnStatefulResult(HoldOnReason reason, string? holdOnMessage)
    {
        if(reason == HoldOnReason.Initialize)
            throw new ArgumentException("HoldOnReason.Initialize must be created using HoldOnStatefulResult.Initialize.", nameof(reason));

        Reason = reason;
        HoldOnMessage = holdOnMessage;
    }

    private HoldOnStatefulResult()
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