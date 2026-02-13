using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Results.Stateful;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Pipeline.CommandInterpreters;

namespace TeleFlow.Core.Pipeline.CommandInterpreters.Stateful;

public class GoToStatefulMiddleware : CommandInterpreterBase<GoToStatefulResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override bool ContinueAfterMatch => false;

    protected override async Task Handle(CommandResultContext<GoToStatefulResult> args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        var session = args.Session;

        session.GoToStep(args.CommandResult.GoToStepNumber);

        if(args.CommandResult.InitializeNextStep)
            session.InitializeStep();

        await _sessionStore.SetAsync(chatId, session);
    }


    public GoToStatefulMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}