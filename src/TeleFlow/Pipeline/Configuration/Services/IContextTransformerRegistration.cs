using TeleFlow.Pipeline;

namespace TeleFlow.Pipeline.Configuration.Services;

public interface IContextTransformerRegistration<TContext, TNextContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TNextContext> next);
}