using System.Collections.ObjectModel;
using TeleFlow.Commands;
using TeleFlow.Models.CommandResults;

namespace TeleFlow.Factories;

public class NavigatedCommandResolver : ICommandFactory<ICommandHandler, NavigateCommandResult>
{
    private readonly ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>> _factories;

    public ICommandHandler Create(NavigateCommandResult context)
    {
        var commandName = context.CommandToNavigate;
        var commandArgs = context.Parameters;

        var factory = _factories.GetValueOrDefault(commandName) ?? throw new Exception("Command Not Found!");

        return factory.Create(commandArgs);
    }

    public NavigatedCommandResolver(ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>> factories)
    {
        _factories = factories;
    }
}