namespace TeleFlow.Builders.Services;

public interface IContextTransformerRegistration<TContext, TNextContext>
{
    IHandler<TContext> Create(IServiceProvider sp, IHandler<TNextContext> next);
}