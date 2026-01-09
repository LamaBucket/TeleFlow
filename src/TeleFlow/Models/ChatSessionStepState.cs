namespace TeleFlow.Models;

public class ChatSessionStepState
{
    public int CurrentCommandStep { get; init; }

    public bool IsStepInitialized { get; init; }

    public ChatSessionStepState(int currentCommandStep, bool isStepInitialized)
    {
        CurrentCommandStep = currentCommandStep;
        IsStepInitialized = isStepInitialized;
    }
}