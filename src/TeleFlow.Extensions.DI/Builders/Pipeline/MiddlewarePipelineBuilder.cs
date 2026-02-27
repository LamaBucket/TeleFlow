using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Extensions.DI.Builders.Pipeline.Services;
using TeleFlow.Extensions.DI.Builders.Pipeline.Services.Registrations.Middlewares;
using TeleFlow.Extensions.DI.Builders.Pipeline.Services.Registrations.Transformers;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Builders.Pipeline;

public class MiddlewarePipelineBuilder
{
    private IContextTransformerRegistration<Update, UpdateContext>? _updateReceiver;
    private IContextTransformerRegistration<UpdateContext, SessionContext>? _commandRouter;
    private IContextTransformerRegistration<SessionContext, CommandResultContext>? _commandExecutor;
    
    private readonly List<IContextMiddlewareRegistration<UpdateContext>> _updateCtxMws = [];
    private readonly List<IContextMiddlewareRegistration<SessionContext>> _sessionMws = [];

    private readonly InterpreterPipelineBuilder _interpreterBuilder = new();


    public InterpreterPipelineBuilder Interpreters => _interpreterBuilder;


    public IHandler<Update> Build(IServiceProvider sp)
    {
        if(_commandExecutor is null)
            throw new InvalidOperationException(
                $"{nameof(MiddlewarePipelineBuilder)} is not configured: command executor is missing. " +
                $"Call {nameof(UseCommandExecutor)}(...) before calling {nameof(Build)}(...).");

        if (_commandRouter is null)
            throw new InvalidOperationException(
                $"{nameof(MiddlewarePipelineBuilder)} is not configured: command router is missing. " +
                $"Call {nameof(UseCommandRouter)}(...) before calling {nameof(Build)}(...).");

        if (_updateReceiver is null)
            throw new InvalidOperationException(
                $"{nameof(MiddlewarePipelineBuilder)} is not configured: update receiver is missing. " +
                $"Call {nameof(UseUpdateReceiver)}(...) before calling {nameof(Build)}(...).");
        


        var interpreters = _interpreterBuilder.Build(sp);
        var exec = _commandExecutor.Create(sp, interpreters);
        var session = Wrap(sp, _sessionMws, exec);
        var routed = _commandRouter.Create(sp, session);
        var update = Wrap(sp, _updateCtxMws, routed);
        var root = _updateReceiver.Create(sp, update);

        return root;
    }

    private static IHandler<TContext> Wrap<TContext>(IServiceProvider sp,
                                                     IEnumerable<IContextMiddlewareRegistration<TContext>> factories,
                                                     IHandler<TContext> next)
    {
        var current = next;
        foreach (var factory in factories.Reverse())
            current = factory.Create(sp, current);
        return current;
    }


    public MiddlewarePipelineBuilder UseUpdateReceiver(IContextTransformerRegistration<Update, UpdateContext> registration)
    { _updateReceiver = registration; return this; }

    public MiddlewarePipelineBuilder UseUpdateReceiver<T>() where T : IHandlerMiddleware<Update, UpdateContext>
        => UseUpdateReceiver(new ContextTransformerTypeRegistration<Update, UpdateContext>(typeof(T)));

    public MiddlewarePipelineBuilder UseUpdateReceiver(Func<IServiceProvider, IHandler<UpdateContext>, IHandler<Update>> factory)
        => UseUpdateReceiver(new ContextTransformerFactoryRegistration<Update, UpdateContext>(factory));
    

    public MiddlewarePipelineBuilder UseCommandRouter(IContextTransformerRegistration<UpdateContext, SessionContext> registration)
    { _commandRouter = registration; return this; }

    public MiddlewarePipelineBuilder UseCommandRouter<T>() where T : IHandlerMiddleware<UpdateContext, SessionContext>
        => UseCommandRouter(new ContextTransformerTypeRegistration<UpdateContext, SessionContext>(typeof(T)));

    public MiddlewarePipelineBuilder UseCommandRouter(Func<IServiceProvider, IHandler<SessionContext>, IHandler<UpdateContext>> factory)
        => UseCommandRouter(new ContextTransformerFactoryRegistration<UpdateContext, SessionContext>(factory));


    public MiddlewarePipelineBuilder UseCommandExecutor(IContextTransformerRegistration<SessionContext, CommandResultContext> registration)
    { _commandExecutor = registration; return this; }
    
    public MiddlewarePipelineBuilder UseCommandExecutor<T>() where T : IHandlerMiddleware<SessionContext, CommandResultContext>
        => UseCommandExecutor(new ContextTransformerTypeRegistration<SessionContext, CommandResultContext>(typeof(T)));

    public MiddlewarePipelineBuilder UseCommandExecutor(Func<IServiceProvider, IHandler<CommandResultContext>, IHandler<SessionContext>> factory)
        => UseCommandExecutor(new ContextTransformerFactoryRegistration<SessionContext, CommandResultContext>(factory));



    public MiddlewarePipelineBuilder WithUpdateContextMiddleware(IContextMiddlewareRegistration<UpdateContext> registration)
    { _updateCtxMws.Add(registration); return this; }

    public MiddlewarePipelineBuilder WithUpdateContextMiddleware<T>() where T : IHandlerMiddleware<UpdateContext>
        => WithUpdateContextMiddleware(new ContextMiddlewareTypeRegistration<UpdateContext>(typeof(T)));

    public MiddlewarePipelineBuilder WithUpdateContextMiddleware(Func<IServiceProvider, IHandler<UpdateContext>, IHandler<UpdateContext>> factory)
        => WithUpdateContextMiddleware(new ContextMiddlewareFactoryRegistration<UpdateContext>(factory));


    public MiddlewarePipelineBuilder WithSessionMiddleware(IContextMiddlewareRegistration<SessionContext> registration)
    { _sessionMws.Add(registration); return this; }

    public MiddlewarePipelineBuilder WithSessionMiddleware<T>() where T : IHandlerMiddleware<SessionContext>
        => WithSessionMiddleware(new ContextMiddlewareTypeRegistration<SessionContext>(typeof(T)));

    public MiddlewarePipelineBuilder WithSessionMiddleware(Func<IServiceProvider, IHandler<SessionContext>, IHandler<SessionContext>> factory)
        => WithSessionMiddleware(new ContextMiddlewareFactoryRegistration<SessionContext>(factory));

        
    public MiddlewarePipelineBuilder()
    {
    }
}