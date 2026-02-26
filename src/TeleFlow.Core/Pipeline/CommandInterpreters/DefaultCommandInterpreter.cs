using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;

namespace TeleFlow.Core.Pipeline.CommandInterpreters;

public class DefaultCommandInterpreter : IHandler<CommandResultContext>
{
    private readonly IChatSessionStore _sessionStore;

    public async Task Handle(CommandResultContext args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        await _sessionStore.RemoveAsync(chatId);

        throw new InvalidOperationException(
            $"No command interpreter matched command result type '{args.CommandResult.GetType()}'. " +
            "Configure an interpreter for this CommandResult (InterpreterPipelineBuilder) or remove the result from the command.");
    }

    public DefaultCommandInterpreter(IChatSessionStore sessionStore)
    {
        _sessionStore = sessionStore;
    }
}