namespace LisBot.Common.Telegram.Models;

public class State<T>
{
    public T CurrentValue { get; init; }


    public State(T defaultValue)
    {
        CurrentValue = defaultValue;
    }
}