namespace TeleFlow.Abstractions.Engine.Commands.Results.Stateful;

public class GoToStatefulResult : CommandResult
{
    public int GoToStepNumber { get; init; }

    public bool InitializeNextStep { get; init; }

    public GoToStatefulResult(int goToStepNumber, bool initializeNextStep)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(goToStepNumber, nameof(goToStepNumber));

        GoToStepNumber = goToStepNumber;
        InitializeNextStep = initializeNextStep;
    }
}