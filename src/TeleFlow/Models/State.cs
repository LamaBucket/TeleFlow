namespace TeleFlow.Models;

public class State<T>
{
    public T CurrentValue { get; set; }


    public State(T defaultValue)
    {
        CurrentValue = defaultValue;
    }
}