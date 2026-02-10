using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Results;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Pipeline.Middlewares.CommandInterpreters;

public class ExitCommandMiddleware : CommandInterpreterBase<ExitCommandResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override async Task Handle(CommandResultContext<ExitCommandResult> args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        await _sessionStore.RemoveAsync(chatId);
    }

    protected override bool ContinueAfterMatch => false;

    public ExitCommandMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}