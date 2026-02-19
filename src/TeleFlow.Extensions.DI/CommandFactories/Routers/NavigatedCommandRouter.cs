using System.Collections.ObjectModel;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Core.Commands.Factories;

namespace TeleFlow.Extensions.DI.CommandFactories.Routers;

public class NavigatedCommandRouter : ICommandFactory<ICommandHandler, NavigateCommandResult>
{
    private readonly ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>> _factories;

    public ICommandHandler Create(NavigateCommandResult context)
    {
        var commandName = context.CommandToNavigate;
        var commandArgs = context.Parameters;

        var factory = _factories.GetValueOrDefault(commandName) ?? throw new Exception("Command Not Found!");

        return factory.Create(commandArgs);
    }

    public NavigatedCommandRouter(ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>> factories)
    {
        _factories = factories;
    }
}