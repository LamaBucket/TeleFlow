namespace LisBot.Common.Telegram.Models;

public class State<T>
{
    public T CurrentValue { get; set; }


    public State(T defaultValue)
    {
        CurrentValue = defaultValue;
    }
}