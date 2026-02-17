using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Extensions.DI.Builders.Pipeline.Services;
using TeleFlow.Extensions.DI.Builders.Pipeline.Services.Registrations.Middlewares;

namespace TeleFlow.Extensions.DI.Builders.Pipeline;

public class InterpreterPipelineBuilder
{
    private readonly List<IContextMiddlewareRegistration<CommandResultContext>> _interpreters = [];

    private Func<IServiceProvider, IHandler<CommandResultContext>>? _terminalInterpreter;


    public IHandler<CommandResultContext> Build(IServiceProvider sp)
    {
        if(_terminalInterpreter is null)
            throw new Exception("Terminal Command Interpreter middleware is not set.");
        
        var current = _terminalInterpreter.Invoke(sp);

        foreach (var factory in _interpreters)
            current = factory.Create(sp, current);

        return current;
    }


    public InterpreterPipelineBuilder UseTerminalCommandInterpreter(Func<IServiceProvider, IHandler<CommandResultContext>> factory)
    {
        _terminalInterpreter = factory;
        return this;
    }

    public InterpreterPipelineBuilder WithInterpreterMiddleware(IContextMiddlewareRegistration<CommandResultContext> registration)
    { _interpreters.Add(registration); return this; }

    public InterpreterPipelineBuilder WithInterpreterMiddleware<T>() where T : IHandlerMiddleware<CommandResultContext>
        => WithInterpreterMiddleware(new ContextMiddlewareTypeRegistration<CommandResultContext>(typeof(T)));

    public InterpreterPipelineBuilder WithInterpreterMiddleware(Func<IServiceProvider, IHandler<CommandResultContext>, IHandler<CommandResultContext>> factory)
        => WithInterpreterMiddleware(new ContextMiddlewareFactoryRegistration<CommandResultContext>(factory));


    public void Clear()
    {
        _interpreters.Clear();
        _terminalInterpreter = null;
    }


    public InterpreterPipelineBuilder Clone()
    {
        List<IContextMiddlewareRegistration<CommandResultContext>> interpretersClone = [.. _interpreters];
        return new(interpretersClone, _terminalInterpreter);
    }


    public InterpreterPipelineBuilder()
    {
    }

    private InterpreterPipelineBuilder(List<IContextMiddlewareRegistration<CommandResultContext>> interpreters, Func<IServiceProvider, IHandler<CommandResultContext>>? terminalInterpreter)
    {
        _interpreters = interpreters;
        _terminalInterpreter = terminalInterpreter;
    }
}