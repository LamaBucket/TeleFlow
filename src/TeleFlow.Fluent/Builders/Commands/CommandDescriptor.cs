using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Core.Commands.Factories;

namespace TeleFlow.Fluent.Builders.Commands;

public delegate ICommandFilter CommandFilterFactory();

internal class CommandDescriptor
{
    public string Name { get; init; }

    public ICommandFactory<ICommandHandler, ChatSession> CommandFactory { get; init; }


    public bool NavigationEnabled { get; private set; }

    public NavigateParametersHandler? NavParamHandler { get; private set; }


    public IReadOnlyList<CommandFilterFactory> Filters => _filters;


    private readonly List<CommandFilterFactory> _filters = [];


    public void EnableNavigation(NavigateParametersHandler? paramHandler = null)
    {
        NavigationEnabled = true;
        NavParamHandler = paramHandler;
    }

    public void AddFilter(CommandFilterFactory filter)
    {
        _filters.Add(filter);
    }

    public CommandDescriptor(string name, ICommandFactory<ICommandHandler, ChatSession> commandFactory)
    {
        Name = name;
        CommandFactory = commandFactory;
    }
}
