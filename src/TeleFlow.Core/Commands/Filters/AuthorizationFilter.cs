using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Core.Commands.Filters;

public delegate Task<bool> AuthorizationProvider(IServiceProvider sp);

public delegate CommandResult AuthorizationFilterCommandResult();

public class AuthorizationFilter : ICommandFilter
{
    private readonly AuthorizationProvider _provider;

    private readonly AuthorizationFilterCommandResult _commandResultProvider;

    public async Task<CommandResult> Execute(UpdateContext context, ICommandHandler next)
    {
        if(await _provider(context.ServiceProvider))
            return await next.Handle(context);
        
        return _commandResultProvider();
    }

    public AuthorizationFilter(AuthorizationProvider provider, AuthorizationFilterCommandResult? commandResultProvider = null)
    {
        _provider = provider;
        _commandResultProvider = commandResultProvider ?? (() => CommandResult.Exit);
    }
}