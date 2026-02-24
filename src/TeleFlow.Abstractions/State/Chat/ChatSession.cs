namespace TeleFlow.Abstractions.State.Chat;

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
        ArgumentOutOfRangeException.ThrowIfNegative(step, nameof(step));

        _currentCommandStep = step;
        _isStepInitialized = false;
    }

    public void InitializeStep() => _isStepInitialized = true;


    public ChatSession(string currentCommand) : this(currentCommand, 0) { }

    public ChatSession(string currentCommand, int currentCommandStep)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentCommand, nameof(currentCommand));

        ArgumentOutOfRangeException.ThrowIfNegative(currentCommandStep, nameof(currentCommandStep));
        
        CurrentCommand = currentCommand;
        _currentCommandStep = currentCommandStep;
        _isStepInitialized = false;
    }
}
