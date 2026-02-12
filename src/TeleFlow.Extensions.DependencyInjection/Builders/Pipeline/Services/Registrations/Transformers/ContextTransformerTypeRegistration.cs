
using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Configuration.Services;

namespace TeleFlow.Pipeline.Configuration.Services.Registrations.Transformers;

public sealed class ContextTransformerTypeRegistration<TContext, TNextContext> : IContextTransformerRegistration<TContext, TNextContext>
{
    public Type MiddlewareType { get; }

    public IHandler<TContext> Create(IServiceProvider sp, IHandler<TNextContext> next)
        => (IHandler<TContext>)ActivatorUtilities.CreateInstance(sp, MiddlewareType, next);

    public ContextTransformerTypeRegistration(Type middlewareType)
    {
        MiddlewareType = middlewareType;
    }
}