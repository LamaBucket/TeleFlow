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

        session ??= new(GetCommandName(args.Update));

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
        var name = GetCommandNameDefault(args);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                $"Unable to determine command name for a new chat session. " +
                $"UpdateType: {args.Type}. " +
                $"Expected a message update with non-empty text.");
        }

        return name;
    }

    private static string? GetCommandNameDefault(Update args)
    {
        return args.Type switch
        {
            UpdateType.Message => args.Message?.Text,
            _ => throw new NotSupportedException($"Update type '{args.Type}' is not supported for command extraction.")
        };
    }


    public SessionExtractionMiddleware(IHandler<SessionContext> next, IChatSessionStore sessionStore)
    {
        Next = next;
        _sessionStore = sessionStore;
    }
}