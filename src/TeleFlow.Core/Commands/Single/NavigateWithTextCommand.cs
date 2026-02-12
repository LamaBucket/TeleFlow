using TeleFlow.Abstractions.Messaging;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Commands.Instant;

public class NavigateWithTextCommand : NavigateCommand
{
    private readonly string _message;

    protected override async Task ExecuteBeforeNavigate(UpdateContext update)
    {
        var messageService = update.GetService<IMessageSender>();

        await messageService.SendMessage(_message);
    }

    public NavigateWithTextCommand(string message, string navigateRoute, Dictionary<string, string>? navigateArgs = null) : base(navigateRoute, navigateArgs)
    {
        _message = message;
    }
}
