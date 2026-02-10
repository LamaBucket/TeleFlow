using TeleFlow.Commands.Interceptors;
using TeleFlow.Commands.Results;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Commands.Decorators;

public class InterceptCommandDecorator : ICommandHandler
{
    private readonly ICommandHandler _inner;

    private readonly ICommandInterceptor _interceptor;
    
    public async Task<CommandResult> Handle(UpdateContext update)
    {
        var interceptResult = await _interceptor.Intercept(update);
        
        if (interceptResult is not null)
            return interceptResult;

        return await _inner.Handle(update);
    }

    public InterceptCommandDecorator(ICommandHandler inner, ICommandInterceptor interceptor)
    {
        _inner = inner;
        _interceptor = interceptor;
    }
}