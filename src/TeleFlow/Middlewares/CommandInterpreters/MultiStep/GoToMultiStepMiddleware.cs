using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;

namespace TeleFlow.Middlewares.CommandInterpreters.MultiStep;

public class GoToMultiStepMiddleware : CommandInterpreterBase<GoToMultiStepResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override bool ContinueAfterMatch => false;

    protected override async Task Handle(CommandResultContext<GoToMultiStepResult> args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        var session = args.Session;

        session.GoToStep(args.CommandResult.GoToStepNumber);

        if(args.CommandResult.NextStepInitialized)
            session.InitializeStep();

        await _sessionStore.SetAsync(chatId, session);
    }


    public GoToMultiStepMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}