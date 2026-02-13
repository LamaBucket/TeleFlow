using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;

namespace TeleFlow.Core.Pipeline.CommandInterpreters;

public class ExitCommandMiddleware : CommandInterpreterBase<ExitCommandResult>
{
    private readonly IChatSessionStateStore _sessionStore;

    protected override async Task Handle(CommandResultContext<ExitCommandResult> args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        await _sessionStore.RemoveAsync(chatId);
    }

    protected override bool ContinueAfterMatch => false;

    public ExitCommandMiddleware(IHandler<CommandResultContext> next, IChatSessionStateStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}