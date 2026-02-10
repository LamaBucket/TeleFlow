using TeleFlow.Abstractions.Messaging;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Results.MultiStep;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Contexts;
using TeleFlow.Pipeline.Middlewares.CommandInterpreters;

namespace TeleFlow.Pipeline.Middlewares.CommandInterpreters.FlowStep;

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