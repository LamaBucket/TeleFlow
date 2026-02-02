using TeleFlow.Interceptors;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;

namespace TeleFlow.Commands;

public class InterceptedCommandWrapper : ICommandHandler
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

    public InterceptedCommandWrapper(ICommandHandler inner, ICommandInterceptor interceptor)
    {
        _inner = inner;
        _interceptor = interceptor;
    }
}