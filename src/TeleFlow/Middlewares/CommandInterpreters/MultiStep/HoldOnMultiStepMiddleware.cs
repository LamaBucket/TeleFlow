using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;
using TeleFlow.Services.Messaging;

namespace TeleFlow.Middlewares.CommandInterpreters.MultiStep;

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

        if(args.CommandResult.HoldOnMessage is not null)
        {
            var messageService = args.GetService<IMessageSender>();
            await messageService.SendMessage(args.CommandResult.HoldOnMessage);
        }

    }

    public HoldOnMultiStepMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}