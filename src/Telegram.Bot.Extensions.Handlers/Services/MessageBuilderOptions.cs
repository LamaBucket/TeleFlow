namespace LisBot.Common.Telegram.Services;

public class MessageBuilderOptions
{
    public int ButtonInRowCount { get; init; }


    public MessageBuilderOptions(int buttonInRowCount)
    {
        if(buttonInRowCount <= 0)
            throw new Exception("The Amount of buttons in one row should be higher than 1");

        ButtonInRowCount = buttonInRowCount;
    }

    public MessageBuilderOptions()
    {
        ButtonInRowCount = int.MaxValue;
    }
}