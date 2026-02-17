using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Extensions.DI.Builders.Pipeline.Services;

namespace TeleFlow.Extensions.DI.Builders.Pipeline.Services.Registrations.Transformers;

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