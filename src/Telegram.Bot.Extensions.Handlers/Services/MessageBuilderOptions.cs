namespace LisBot.Common.Telegram.Services;

public class MessageBuilderOptions
{
    public bool ClearOnBuild { get; init; }

    public int ButtonInRowCount { get; init; }


    public MessageBuilderOptions(bool clearOnBuild, int buttonInRowCount)
    {
        if(buttonInRowCount <= 0)
            throw new Exception("The Amount of buttons in one row should be higher than 1");

        ClearOnBuild = clearOnBuild;
        ButtonInRowCount = buttonInRowCount;
    }

    public MessageBuilderOptions()
    {
        ClearOnBuild = true;
        ButtonInRowCount = int.MaxValue;
    }
}