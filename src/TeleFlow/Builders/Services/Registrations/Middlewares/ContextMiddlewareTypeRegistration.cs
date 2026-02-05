
using Microsoft.Extensions.DependencyInjection;

namespace TeleFlow.Builders.Services.Registrations.Middlewares;

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