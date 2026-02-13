using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Pipeline;

namespace TeleFlow.Extensions.DependencyInjection.Builders.Pipeline.Services.Registrations.Middlewares;

public sealed class ContextMiddlewareTypeRegistration<TContext> : IContextMiddlewareRegistration<TContext>
{
    public Type MiddlewareType { get; }

    public ContextMiddlewareTypeRegistration(Type middlewareType)
    {
        MiddlewareType = middlewareType;
    }

    public IHandler<TContext> Create(IServiceProvider sp, IHandler<TContext> next)
        => (IHandler<TContext>)ActivatorUtilities.CreateInstance(sp, MiddlewareType, next);
}