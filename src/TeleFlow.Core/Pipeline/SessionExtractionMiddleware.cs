using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Core.Transport.Callbacks;
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

        session ??= new(GetCommandName(args));

        SessionContext context = new(session, args);

        await Next.Handle(context);
    }

    private async Task<ChatSession?> GetCurrentSession(UpdateContext args)
    {
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        return await _sessionStore.GetAsync(chatId);
    }

    protected virtual string GetCommandName(UpdateContext args)
    {
        var update = args.Update;
        var name = GetCommandNameDefault(args);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                $"Unable to determine command name for a new chat session. " +
                $"UpdateType: {update.Type}. ");
        }

        return name;
    }

    private static string? GetCommandNameDefault(UpdateContext args)
    {
        var update = args.Update;

        return update.Type switch
        {
            UpdateType.Message => update.Message?.Text,
            UpdateType.CallbackQuery => GetCommandNameFromCallbackQuery(args),
            _ => throw new NotSupportedException($"Update type '{update.Type}' is not supported for command extraction.")
        };
    }

    private static string? GetCommandNameFromCallbackQuery(UpdateContext args)
    {
        var update = args.Update;

        if(update.CallbackQuery is null || update.CallbackQuery.Data is null)
            return null;
        
        var codec = args.GetService<ICallbackCodec>();
        var actionParser = args.GetService<ICallbackActionParser>();

        if(!codec.TryDecodeAction(update.CallbackQuery.Data, out var token))
            return null;
        
        if(!actionParser.TryParse(token, out var action))
            return null;
        
        if(action is not CallbackAction.CommandAction.Execute execute)
            return null;
        
        return execute.CommandKey;
    }


    public SessionExtractionMiddleware(IHandler<SessionContext> next, IChatSessionStore sessionStore)
    {
        Next = next;
        _sessionStore = sessionStore;
    }
}