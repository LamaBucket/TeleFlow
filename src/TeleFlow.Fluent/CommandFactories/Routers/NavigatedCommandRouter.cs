using System.Collections.ObjectModel;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Core.Commands.Factories;

namespace TeleFlow.Fluent.CommandFactories.Routers;

public class NavigatedCommandRouter : ICommandFactory<ICommandHandler, NavigateCommandResult>
{
    private readonly ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>> _factories;

    public ICommandHandler Create(NavigateCommandResult context)
    {
        var commandName = context.CommandToNavigate;
        var commandArgs = context.Parameters;

        ArgumentException.ThrowIfNullOrWhiteSpace(commandName, nameof(commandName));

        //TODO Maybe use TeleFlowException instead of KeyNotFound cause this can happen because of user?

        var factory = _factories.GetValueOrDefault(commandName) ?? throw new KeyNotFoundException(
                $"{nameof(NavigatedCommandRouter)}: command '{commandName}' was not found. " +
                $"Available commands: [{string.Join(", ", _factories.Keys.OrderBy(k => k))}].");

        return factory.Create(commandArgs);
    }

    public NavigatedCommandRouter(ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>> factories)
    {
        _factories = factories;
    }
}