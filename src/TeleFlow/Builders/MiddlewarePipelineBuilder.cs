using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Commands;
using TeleFlow.Factories;
using TeleFlow.Middlewares;
using TeleFlow.Middlewares.CommandInterpreters;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;
using Telegram.Bot.Types;

namespace TeleFlow.Builders;

public class MiddlewarePipelineBuilder
{
    private Func<IHandler<UpdateContext>, IHandlerMiddleware<Update, UpdateContext>>? _updateReceiver;
    private Func<IHandler<SessionContext>, IHandlerMiddleware<UpdateContext, SessionContext>>? _commandRouter;
    private Func<IHandler<CommandResultContext>, IHandlerMiddleware<SessionContext, CommandResultContext>>? _commandExecutor;
    
    private readonly List<Func<IHandler<UpdateContext>, IHandlerMiddleware<UpdateContext>>> _updateCtxMws = [];
    private readonly List<Func<IHandler<SessionContext>, IHandlerMiddleware<SessionContext>>> _sessionMws = [];
    private readonly List<Func<IHandler<CommandResultContext>, IHandlerMiddleware<CommandResultContext>>> _resultMws = [];

    private readonly IChatSessionStore? _chatSessionStore;


    public IHandler<Update> Build()
    {
        IHandler<CommandResultContext> tail = GenerateTerminalHandler();
        var result = Wrap(_resultMws, tail);
        var exec = _commandExecutor?.Invoke(result) ?? throw new Exception("Command Executor middleware is not set.");
        var session = Wrap(_sessionMws, exec);
        var routed = _commandRouter?.Invoke(session) ?? throw new Exception("Command Router middleware is not set.");
        var update = Wrap(_updateCtxMws, routed);
        var root = _updateReceiver?.Invoke(update) ?? throw new Exception("Update Receiver middleware is not set.");

        return root;
    }

    protected virtual IHandler<CommandResultContext> GenerateTerminalHandler()
    {
        if(_chatSessionStore is null)
            throw new InvalidOperationException("Default Command Interpreter is supported only with ChatSessionStore provided. Either provide ChatSessionStore to the builder or use custom terminal handler.");

        return new DefaultCommandInterpreter(_chatSessionStore);
    }

    private static IHandler<TContext> Wrap<TContext>(IEnumerable<Func<IHandler<TContext>, IHandlerMiddleware<TContext>>> factories, IHandler<TContext> next)
    {
        var current = next;
        foreach (var factory in factories.Reverse())
            current = factory(current);
        return current;
    }


    public void WithUpdateReceiver(Func<IHandler<UpdateContext>, IHandlerMiddleware<Update, UpdateContext>> factory)
    {
        _updateReceiver = factory;
    }

    public void WithCommandRouter(Func<IHandler<SessionContext>, IHandlerMiddleware<UpdateContext, SessionContext>> factory)
    {
        _commandRouter = factory;
    }

    public void WithCommandExecutor(Func<IHandler<CommandResultContext>, IHandlerMiddleware<SessionContext, CommandResultContext>> factory)
    {
        _commandExecutor = factory;
    }


    public void UseUpdateContextMiddleware(Func<IHandler<UpdateContext>, IHandlerMiddleware<UpdateContext>> factory, bool prepend = false)
    {
        _updateCtxMws.Insert(prepend ? 0 : _updateCtxMws.Count, factory);
    }

    public void UseSessionMiddleware(Func<IHandler<SessionContext>, IHandlerMiddleware<SessionContext>> factory, bool prepend = false)
    {
        _sessionMws.Insert(prepend ? 0 : _sessionMws.Count, factory);
    }

    public void UseCommandResultMiddleware(Func<IHandler<CommandResultContext>, IHandlerMiddleware<CommandResultContext>> factory, bool prepend = false)
    {
        _resultMws.Insert(prepend ? 0 : _resultMws.Count, factory);
    }

    public void UseCommandResultMiddleware(Func<IHandler<CommandResultContext>, IChatSessionStore, IHandlerMiddleware<CommandResultContext>> factory, bool prepend = false)
    {
        if(_chatSessionStore == null)
            throw new InvalidOperationException("Chat session store is not provided in the builder.");

        _resultMws.Insert(prepend ? 0 : _resultMws.Count, (next) => factory.Invoke(next, _chatSessionStore));
    }

    
    public MiddlewarePipelineBuilder(IServiceScopeFactory? serviceScopeFactory = null, IChatSessionStore? chatSessionStore = null, ICommandFactory<ICommandHandler, ChatSession>? commandFactory = null)
    {
        _chatSessionStore = chatSessionStore;

        if(serviceScopeFactory != null)
            _updateReceiver = next => new UpdateReceiverMiddleware(next, serviceScopeFactory);
        
        if(chatSessionStore != null)
            _commandRouter = next => new CommandRoutingMiddleware(next, chatSessionStore);

        if(commandFactory != null)
            _commandExecutor = next => new CommandExecutionMiddleware(next, commandFactory);
    }
}