using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Results.Stateful;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Pipeline.CommandInterpreters;

namespace TeleFlow.Core.Pipeline.CommandInterpreters.Stateful;

public class HoldOnStatefulMiddleware : CommandInterpreterBase<HoldOnStatefulResult>
{
    private readonly IChatSessionStore _sessionStore;

    protected override bool ContinueAfterMatch => false;

    protected override async Task Handle(CommandResultContext<HoldOnStatefulResult> args)
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

    public HoldOnStatefulMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore) : base(next)
    {
        _sessionStore = sessionStore;
    }
}