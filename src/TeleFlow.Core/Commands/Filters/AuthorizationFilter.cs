using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Filters;

public delegate Task<bool> AuthorizationProvider(IServiceProvider sp);

public delegate CommandResult AuthorizationFilterCommandResult();

public class AuthorizationFilter : ICommandFilter
{
    private readonly AuthorizationProvider _provider;

    private readonly AuthorizationFilterCommandResult _commandResultProvider;

    private readonly string? _forbidMessage;

    public async Task<CommandResult> Execute(UpdateContext context, ICommandHandler next)
    {
        if(await _provider(context.ServiceProvider))
            return await next.Handle(context);

        if(_forbidMessage is not null)
        {
            var msgService = context.GetService<IMessageSendService>();
            await msgService.SendMessage(_forbidMessage);
        }
        
        return _commandResultProvider();
    }

    public AuthorizationFilter(AuthorizationProvider provider, AuthorizationFilterCommandResult? commandResultProvider = null, string? forbidMessage = null)
    {
        _provider = provider;
        _commandResultProvider = commandResultProvider ?? (() => CommandResult.Exit);
        _forbidMessage = forbidMessage;
    }
}