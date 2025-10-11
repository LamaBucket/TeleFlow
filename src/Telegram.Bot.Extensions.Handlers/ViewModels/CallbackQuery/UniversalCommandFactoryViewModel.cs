namespace Telegram.Bot.Extensions.Handlers.ViewModels.CallbackQuery;

public class UniversalCommandFactoryViewModel
{
    public string CommandToExecute { get; init; }

    public UniversalCommandFactoryViewModel(string commandToExecute)
    {
        CommandToExecute = commandToExecute;
    }
}