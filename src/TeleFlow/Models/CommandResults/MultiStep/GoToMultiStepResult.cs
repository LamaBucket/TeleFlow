using TeleFlow.Models.MultiStep;

namespace TeleFlow.Models.CommandResults;

public class GoToMultiStepResult : CommandResult
{
    public int GoToStepNumber { get; init; }

    public GoToMultiStepResult(int goToStepNumber)
    {
        if(goToStepNumber < 0)
            throw new Exception("");

        GoToStepNumber = goToStepNumber;
    }
}