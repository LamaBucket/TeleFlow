using TeleFlow.Abstractions.Engine.Commands.Interceptors;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Core.Commands.Decorators;

public class InterceptCommandStepDecorator : ICommandStep
{
    private readonly ICommandStep _inner;

    private readonly ICommandStepInterceptor _interceptor;
    
    public async Task<CommandStepResult> Handle(UpdateContext args)
    {
        var interceptResult = await _interceptor.InterceptBeforeStep(args);

        if(interceptResult is not null)
            return interceptResult;
        
        var stepResult = await _inner.Handle(args);

        return await _interceptor.InterceptAfterStep(args, stepResult);
    }

    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        await _interceptor.InterceptBeforeOnEnter(serviceProvider);
        await _inner.OnEnter(serviceProvider);
        await _interceptor.InterceptAfterOnEnter(serviceProvider);
    }

    public InterceptCommandStepDecorator(ICommandStep inner, ICommandStepInterceptor interceptor)
    {
        _inner = inner;
        _interceptor = interceptor;
    }
}