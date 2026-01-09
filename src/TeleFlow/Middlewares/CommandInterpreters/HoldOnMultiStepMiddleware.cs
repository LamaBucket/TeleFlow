using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;

namespace TeleFlow.Middlewares.CommandInterpreters;

public class HoldOnMultiStepMiddleware : CommandInterpreterBase<HoldOnMultiStepResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override bool ContinueAfterMatch => false;

    protected override async Task Handle(CommandResultContext<HoldOnMultiStepResult> args)
    {
        var holdOnReason = args.CommandResult.Reason;

        if(holdOnReason == HoldOnReason.Initialize)
        {
            var chatId = args.GetService<IChatIdProvider>().GetChatId();
            var session = args.Session;
            session.InitializeStep();

            await _sessionStore.SetAsync(chatId, session);
        }

    }

    public HoldOnMultiStepMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}