using TeleFlow.Middlewares.CommandInterpreters;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;

namespace TeleFlow;

public class ExitCommandMiddleware : CommandInterpreterBase<ExitCommandResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override async Task Handle(CommandResultContext<ExitCommandResult> args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        await _sessionStore.RemoveAsync(chatId);
    }

    protected override bool ContinueAfterMatch => false;

    public ExitCommandMiddleware(IChatSessionStore sessionStore, IHandler<CommandResultContext> next) : base(next)
    {
        _sessionStore = sessionStore;
    }
}