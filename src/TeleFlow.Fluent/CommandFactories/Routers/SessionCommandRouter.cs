using System.Collections.ObjectModel;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Factories;

namespace TeleFlow.Fluent.CommandFactories.Routers;

public class SessionCommandRouter : ICommandFactory<ICommandHandler, ChatSession>
{
    private readonly ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, ChatSession>> _factories;

    public ICommandHandler Create(ChatSession context)
    {
        string commandName = context.CurrentCommand;

        //TODO Maybe use TeleFlowException instead of KeyNotFound cause this can happen because of user?

        if(!_factories.TryGetValue(commandName, out ICommandFactory<ICommandHandler, ChatSession>? value))
            throw new KeyNotFoundException(
                $"{nameof(SessionCommandRouter)}: command '{commandName}' was not found. " +
                $"Available commands: [{string.Join(", ", _factories.Keys.OrderBy(k => k))}].");

        return value.Create(context);
    }

    public SessionCommandRouter(ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, ChatSession>> factories)
    {
        _factories = factories;
    }
}