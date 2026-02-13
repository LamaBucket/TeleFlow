using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateless;

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
