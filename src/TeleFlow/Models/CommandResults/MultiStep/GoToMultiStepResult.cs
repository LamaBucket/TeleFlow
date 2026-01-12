using TeleFlow.Models.MultiStep;

namespace TeleFlow.Models.CommandResults;

public class GoToMultiStepResult : CommandResult
{
    public int GoToStepNumber { get; init; }

    public bool NextStepInitialized { get; init; }

    public GoToMultiStepResult(int goToStepNumber, bool nextStepInitialized)
    {
        if (goToStepNumber < 0)
            throw new Exception("");

        GoToStepNumber = goToStepNumber;
        NextStepInitialized = nextStepInitialized;
    }
}