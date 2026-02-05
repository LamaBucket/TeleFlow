namespace TeleFlow.Builders.Services.Registrations.Middlewares;

public sealed class ContextMiddlewareFactoryRegistration<TContext> : IContextMiddlewareRegistration<TContext>
{
    private readonly Func<IServiceProvider, IHandler<TContext>, IHandler<TContext>> _factory;

    public ContextMiddlewareFactoryRegistration(Func<IServiceProvider, IHandler<TContext>, IHandler<TContext>> factory)
    {
        _factory = factory;
    }

    public IHandler<TContext> Create(IServiceProvider sp, IHandler<TContext> next)
        => _factory(sp, next);
}