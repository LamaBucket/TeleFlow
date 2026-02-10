namespace TeleFlow.Abstractions.Sessions;

public class ChatSessionStepSnapshot
{
    public int CurrentCommandStep { get; init; }

    public bool IsStepInitialized { get; init; }

    public ChatSessionStepSnapshot(int currentCommandStep, bool isStepInitialized)
    {
        CurrentCommandStep = currentCommandStep;
        IsStepInitialized = isStepInitialized;
    }
}