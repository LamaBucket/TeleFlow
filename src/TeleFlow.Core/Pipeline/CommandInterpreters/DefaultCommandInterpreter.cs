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

        throw new Exception("No command interpreter matched the command result of type " + args.CommandResult.GetType().FullName);
    }

    public DefaultCommandInterpreter(IChatSessionStore sessionStore)
    {
        _sessionStore = sessionStore;
    }
}