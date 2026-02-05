namespace TeleFlow.Builders.Services;

public interface IContextMiddlewareRegistration<TContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TContext> next);
}