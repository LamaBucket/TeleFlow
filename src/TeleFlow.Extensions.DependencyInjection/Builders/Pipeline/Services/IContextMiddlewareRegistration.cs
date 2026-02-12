using TeleFlow.Pipeline;

namespace TeleFlow.Pipeline.Configuration.Services;

public interface IContextMiddlewareRegistration<TContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TContext> next);
}