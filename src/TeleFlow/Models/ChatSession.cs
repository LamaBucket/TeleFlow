namespace TeleFlow.Models;

public class ChatSession
{
    public string CurrentCommand { get; init; }


    public int CurrentCommandStep => _currentCommandStep;

    public bool IsStepInitialized => _isStepInitialized;


    private int _currentCommandStep;

    private bool _isStepInitialized;


    public void GoToStep(int step)
    {
        if(step < 0)
            throw new Exception("step cannot be less than 0");
        
        _currentCommandStep = step;
        _isStepInitialized = false;
    }

    public void InitializeStep()
    {
        _isStepInitialized = true;
    }

    public ChatSession(string currentCommand) : this(currentCommand, 0)
    {
        
    }

    public ChatSession(string currentCommand, int currentCommandStep)
    {
        CurrentCommand = currentCommand;

        if(currentCommandStep < 0)
            throw new Exception("currentCommandStep cannot be less than 0");

        _currentCommandStep = currentCommandStep;
        _isStepInitialized = false;
    }
}
