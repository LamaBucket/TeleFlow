using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;

namespace TeleFlow.Commands.Stateless;

public class NavigateCommand : ICommandHandler
{
    private readonly string _navigateRoute;

    private readonly Dictionary<string, string> _navigateArgs;

    public async Task<CommandResult> Handle(UpdateContext update)
    {
        await ExecuteBeforeNavigate(update);

        return new NavigateCommandResult(_navigateRoute, _navigateArgs);
    }

    protected virtual async Task ExecuteBeforeNavigate(UpdateContext update)
    {
        
    }

    public NavigateCommand(string navigateRoute, Dictionary<string, string>? navigateArgs = null)
    {
        _navigateRoute = navigateRoute;
        _navigateArgs = navigateArgs ?? [];
    }
}
