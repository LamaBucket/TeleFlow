using System.Collections.ObjectModel;
using TeleFlow.Commands;
using TeleFlow.Models;

namespace TeleFlow.Factories;

public class SessionCommandResolver : ICommandFactory<ICommandHandler, ChatSession>
{
    private readonly ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, ChatSession>> _factories;

    public ICommandHandler Create(ChatSession context)
    {
        string commandName = context.CurrentCommand;

        if(!_factories.TryGetValue(commandName, out ICommandFactory<ICommandHandler, ChatSession>? value))
            throw new Exception($"Command {commandName} Not Found!");

        return value.Create(context);
    }

    public SessionCommandResolver(ReadOnlyDictionary<string, ICommandFactory<ICommandHandler, ChatSession>> factories)
    {
        _factories = factories;
    }
}