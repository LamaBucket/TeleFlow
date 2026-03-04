using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Core.Commands.Decorators;

public delegate Task NavigateParametersHandler(NavigateCommandParameters navigateArgs, IServiceProvider sp);

public class NavigateCommandDecorator : ICommandHandler 
//Command is wrapped in NavigatedCommandWrapper ONLY on navigation, further updates are being handled by the main Middleware.
//Thats why we DO NOT check for paramHandler being already handled here.
{
    private readonly ICommandHandler _inner;

    private readonly NavigateCommandParameters _args;

    private readonly NavigateParametersHandler? _paramHandler;


    public async Task<CommandResult> Handle(UpdateContext update)
    {
        if(_paramHandler is not null)
            await _paramHandler.Invoke(_args, update.ServiceProvider); 

        return await _inner.Handle(update);
    }

    public NavigateCommandDecorator(ICommandHandler inner, NavigateCommandParameters args, NavigateParametersHandler? paramHandler = null)
    {
        _inner = inner;
        _args = args;
        _paramHandler = paramHandler;
    }
}