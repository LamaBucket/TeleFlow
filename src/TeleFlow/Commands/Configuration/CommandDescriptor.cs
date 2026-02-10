using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleFlow.Commands;
using TeleFlow.Factories;
using TeleFlow.Interceptors;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;

namespace TeleFlow.Builders;

internal class CommandDescriptor
{
    public string Name { get; init; }

    public ICommandFactory<ICommandHandler, ChatSession> CommandFactory { get; init; }


    public bool NavigationEnabled { get; private set; }

    public Func<NavigateCommandParameters, IServiceProvider, Task>? NavParamHandler { get; private set; }


    public IReadOnlyList<Func<ICommandInterceptor>> Interceptors => _interceptors;


    private readonly List<Func<ICommandInterceptor>> _interceptors = new();


    public void EnableNavigation(Func<NavigateCommandParameters, IServiceProvider, Task>? paramHandler = null)
    {
        NavigationEnabled = true;
        NavParamHandler = paramHandler;
    }

    public void AddInterceptor(Func<ICommandInterceptor> interceptorFactory)
    {
        _interceptors.Add(interceptorFactory);
    }

    public CommandDescriptor(string name, ICommandFactory<ICommandHandler, ChatSession> commandFactory)
    {
        Name = name;
        CommandFactory = commandFactory;
    }
}
