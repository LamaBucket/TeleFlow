using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Factories;

namespace TeleFlow.Extensions.DI.Builders.Commands;

internal class CommandDescriptor
{
    public string Name { get; init; }

    public ICommandFactory<ICommandHandler, ChatSession> CommandFactory { get; init; }


    public bool NavigationEnabled { get; private set; }

    public Func<NavigateCommandParameters, IServiceProvider, Task>? NavParamHandler { get; private set; }


    public IReadOnlyList<Func<ICommandFilter>> Filters => _filters;


    private readonly List<Func<ICommandFilter>> _filters = [];


    public void EnableNavigation(Func<NavigateCommandParameters, IServiceProvider, Task>? paramHandler = null)
    {
        NavigationEnabled = true;
        NavParamHandler = paramHandler;
    }

    public void AddFilter(Func<ICommandFilter> filter)
    {
        _filters.Add(filter);
    }

    public CommandDescriptor(string name, ICommandFactory<ICommandHandler, ChatSession> commandFactory)
    {
        Name = name;
        CommandFactory = commandFactory;
    }
}
