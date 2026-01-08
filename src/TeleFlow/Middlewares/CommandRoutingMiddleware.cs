using TeleFlow.Models;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;
using Telegram.Bot.Types;

namespace TeleFlow.Middlewares;

public class CommandRoutingMiddleware : IHandlerMiddleware<UpdateContext, SessionContext>
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
        return args.GetCommandName() ?? throw new Exception("Command name could not be determined");
    }


    public CommandRoutingMiddleware(IHandler<SessionContext> next, IChatSessionStore sessionStore)
    {
        Next = next;
        _sessionStore = sessionStore;
    }
}