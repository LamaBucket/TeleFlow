using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Results.MultiStep;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Pipeline.Middlewares.CommandInterpreters.FlowStep;

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