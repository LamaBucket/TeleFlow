using TeleFlow.Pipeline.Configuration.Services;
using TeleFlow.Pipeline.Configuration.Services.Registrations.Middlewares;
using TeleFlow.Pipeline.Configuration.Services.Registrations.Transformers;
using TeleFlow.Pipeline.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Pipeline.Configuration;

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
        var interpreters = _interpreterBuilder.Build(sp);
        var exec = _commandExecutor?.Create(sp, interpreters) ?? throw new Exception("Command Executor middleware is not set.");
        var session = Wrap(sp, _sessionMws, exec);
        var routed = _commandRouter?.Create(sp, session) ?? throw new Exception("Command Router middleware is not set.");
        var update = Wrap(sp, _updateCtxMws, routed);
        var root = _updateReceiver?.Create(sp, update) ?? throw new Exception("Update Receiver middleware is not set.");

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