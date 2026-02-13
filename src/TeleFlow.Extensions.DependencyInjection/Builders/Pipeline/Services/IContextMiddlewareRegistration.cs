using TeleFlow.Abstractions.Engine.Pipeline;

namespace TeleFlow.Extensions.DependencyInjection.Builders.Pipeline.Services;

public interface IContextMiddlewareRegistration<TContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TContext> next);
}