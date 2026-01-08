using TeleFlow.Middlewares.CommandInterpreters;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;

namespace TeleFlow;

public class ContinueCommandMiddleware : CommandInterpreterBase<ContinueCommandResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override async Task Handle(CommandResultContext<ContinueCommandResult> args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        var session = args.Session;

        session.MoveNextStep();

        await _sessionStore.SetAsync(chatId, session);
    }

    protected override bool ContinueAfterMatch => false;


    public ContinueCommandMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}