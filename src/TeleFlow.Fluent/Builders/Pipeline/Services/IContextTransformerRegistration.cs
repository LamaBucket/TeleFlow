using TeleFlow.Abstractions.Engine.Pipeline;

namespace TeleFlow.Fluent.Builders.Pipeline.Services;

public interface IContextTransformerRegistration<TContext, TNextContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TNextContext> next);
}