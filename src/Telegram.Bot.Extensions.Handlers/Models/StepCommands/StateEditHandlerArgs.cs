namespace LisBot.Common.Telegram.Models.StepCommands;

public class StateEditHandlerArgs<T> where T : Enum
{
    public T PropertyToEdit { get; init; }

    public StateEditHandlerArgs(T propertyToEdit)
    {
        PropertyToEdit = propertyToEdit;
    }
}