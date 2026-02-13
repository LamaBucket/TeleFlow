namespace TeleFlow.Abstractions.State.ChatSession;

public sealed class ChatSession
{
    public readonly record struct ChatSessionStepSnapshot(int CurrentCommandStep, bool IsStepInitialized);

    public string CurrentCommand { get; }

    public int CurrentCommandStep => _currentCommandStep;

    public bool IsStepInitialized => _isStepInitialized;

    private int _currentCommandStep;

    private bool _isStepInitialized;


    public ChatSessionStepSnapshot CreateSnapshot() => new(_currentCommandStep, _isStepInitialized);

    public void GoToStep(int step)
    {
        if (step < 0)
            throw new ArgumentOutOfRangeException(nameof(step), step, "Step cannot be less than 0.");

        _currentCommandStep = step;
        _isStepInitialized = false;
    }

    public void InitializeStep() => _isStepInitialized = true;


    public ChatSession(string currentCommand) : this(currentCommand, 0) { }

    public ChatSession(string currentCommand, int currentCommandStep)
    {
        if (string.IsNullOrWhiteSpace(currentCommand))
            throw new ArgumentException("Command name cannot be null or whitespace.", nameof(currentCommand));

        if (currentCommandStep < 0)
            throw new ArgumentOutOfRangeException(nameof(currentCommandStep), currentCommandStep, "Step cannot be less than 0.");

        CurrentCommand = currentCommand;
        _currentCommandStep = currentCommandStep;
        _isStepInitialized = false;
    }
}
