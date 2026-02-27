using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Interceptors;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Core.Commands.Decorators;

public class InterceptCommandDecorator : ICommandHandler
{
    private readonly ICommandHandler _inner;

    private readonly ICommandInterceptor _interceptor;
    
    public async Task<CommandResult> Handle(UpdateContext update)
    {
        var interceptResult = await _interceptor.InterceptBeforeCommand(update);
        
        if (interceptResult is not null)
            return interceptResult;

        var commandResult = await _inner.Handle(update);

        return await _interceptor.InterceptAfterCommand(update, commandResult);
    }

    public InterceptCommandDecorator(ICommandHandler inner, ICommandInterceptor interceptor)
    {
        _inner = inner;
        _interceptor = interceptor;
    }
}