using TeleFlow.Commands;
using TeleFlow.Models;

namespace TeleFlow.Factories.CommandFactories;

public class GenericCommandFactory :
    ICommandFactory<IOutputOnlyCommand, ConversationContext>,
    ICommandFactory<ICommandHandler, ConversationContext>
{
    private readonly Dictionary<string, Func<ConversationContext, IOutputOnlyCommand>> _outputOnlyCommandFactories;

    private readonly Dictionary<string, Func<ConversationContext, ICommandHandler>> _commandHandlerFactories;


    public ICommandHandler Create(ConversationContext context)
    {
        return CreateCommandHandler(context) ?? throw new InvalidOperationException();
    }

    IOutputOnlyCommand ICommandFactory<IOutputOnlyCommand, ConversationContext>.Create(ConversationContext context)
    {
        var command = CreateOutputCommand(context);

        command ??= CreateCommandHandler(context);
        
        return command ?? throw new InvalidOperationException();
    }

    private ICommandHandler? CreateCommandHandler(ConversationContext context)
    {
        var currentCommand = context.SessionInfo.CurrentCommand;

        if (_commandHandlerFactories.TryGetValue(currentCommand, out var factory))
        {
            return factory(context);
        }

        return null;
    }

    private IOutputOnlyCommand? CreateOutputCommand(ConversationContext context)
    {
        var currentCommand = context.SessionInfo.CurrentCommand;

        if (_outputOnlyCommandFactories.TryGetValue(currentCommand, out var factory))
        {
            return factory(context);
        }

        return null;
    }
}