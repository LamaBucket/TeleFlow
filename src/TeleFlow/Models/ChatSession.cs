namespace TeleFlow.Models;

public class ChatSession
{
    public string CurrentCommand { get; init; }


    public int CurrentCommandStep => _currentCommandStep;

    private int _currentCommandStep;


    public void MoveNextStep()
    {
        _currentCommandStep += 1;
    }


    public ChatSession(string currentCommand) : this(currentCommand, 0)
    {
        
    }

    public ChatSession(string currentCommand, int currentCommandStep)
    {
        CurrentCommand = currentCommand;
        _currentCommandStep = currentCommandStep;
    }
}
