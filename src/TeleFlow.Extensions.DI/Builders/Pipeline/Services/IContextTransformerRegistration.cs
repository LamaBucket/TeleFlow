using TeleFlow.Abstractions.Engine.Pipeline;

namespace TeleFlow.Extensions.DI.Builders.Pipeline.Services;

public interface IContextTransformerRegistration<TContext, TNextContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TNextContext> next);
}