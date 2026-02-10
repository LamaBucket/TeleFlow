using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Configuration.Services;

namespace TeleFlow.Pipeline.Configuration.Services.Registrations.Transformers;

public sealed class ContextTransformerFactoryRegistration<TContext, TNextContext> : IContextTransformerRegistration<TContext, TNextContext>
{
    private readonly Func<IServiceProvider, IHandler<TNextContext>, IHandler<TContext>> _factory;

    public IHandler<TContext> Create(IServiceProvider sp, IHandler<TNextContext> next)
        => _factory(sp, next);

    public ContextTransformerFactoryRegistration(Func<IServiceProvider, IHandler<TNextContext>, IHandler<TContext>> factory)
    {
        _factory = factory;
    }
}