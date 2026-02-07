using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Models.Contexts;
using TeleFlow.Services.Messaging;

namespace TeleFlow.Commands.Stateless;

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
