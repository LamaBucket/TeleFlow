using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Pipeline;

public class SessionExtractionMiddleware : IHandlerMiddleware<UpdateContext, SessionContext>
{
    public IHandler<SessionContext> Next { get; init; }

    private readonly IChatSessionStore _sessionStore;

    public async Task Handle(UpdateContext args)
    {
        var session = await GetCurrentSession(args);

        if(session is null)
            session = new(GetCommandName(args.Update));

        SessionContext context = new(session, args);

        await Next.Handle(context);
    }

    private async Task<ChatSession?> GetCurrentSession(UpdateContext args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        return await _sessionStore.GetAsync(chatId);
    }

    protected virtual string GetCommandName(Update args)
    {
        return GetCommandNameDefault(args) ?? throw new Exception("Command name could not be determined");
    }

    private static string? GetCommandNameDefault(Update args)
    {
        switch(args.Type)
        {
            case UpdateType.Message:
                return args.Message?.Text;
        }
        throw new InvalidOperationException("Unknown update type");
    }


    public SessionExtractionMiddleware(IHandler<SessionContext> next, IChatSessionStore sessionStore)
    {
        Next = next;
        _sessionStore = sessionStore;
    }
}