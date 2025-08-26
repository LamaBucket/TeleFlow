namespace LisBot.Common.Telegram.ViewModels.CallbackQuery;

public class UniversalCommandFactoryViewModel
{
    public string CommandToExecute { get; init; }

    public UniversalCommandFactoryViewModel(string commandToExecute)
    {
        CommandToExecute = commandToExecute;
    }
}