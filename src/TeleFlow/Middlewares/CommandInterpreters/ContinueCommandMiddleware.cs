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
        var session = args.Session;

        session.MoveNextStep();

        await _sessionStore.SetAsync(session);
    }

    protected override bool ContinueAfterMatch => true;


    public ContinueCommandMiddleware(IChatSessionStore sessionStore, IHandler<CommandResultContext> next) : base(next)
    {
        _sessionStore = sessionStore;
    }
}