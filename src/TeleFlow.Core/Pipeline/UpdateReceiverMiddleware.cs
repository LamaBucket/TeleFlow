using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Pipeline;

public class UpdateReceiverMiddleware : IHandlerMiddleware<Update, UpdateContext>
{
    public IHandler<UpdateContext> Next { get; init; }

    private readonly IServiceScopeFactory _serviceScopeFactory;


    public async Task Handle(Update args)
    {
        var chatId = GetChatId(args);

        using var scope = CreateScope(chatId);

        UpdateContext context = new(args, scope.ServiceProvider);
        await Next.Handle(context);
    }


    protected virtual long GetChatId(Update args)
    {
        return GetChatIdDefault(args);
    }

    private static long GetChatIdDefault(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => GetChatIdFromMessage(update),
            UpdateType.CallbackQuery => GetChatIdFromCallbackQuery(update),
            _ => throw new NotSupportedException(
                $"Update type '{update.Type}' is not supported for chat id extraction.")
        };
    }

    private static long GetChatIdFromMessage(Update update)
    {
        var chat = (update.Message?.Chat) ?? throw new InvalidOperationException("Unable to extract chat id: update is of type 'Message' but Message.Chat is missing.");
        return chat.Id;
    }

    private static long GetChatIdFromCallbackQuery(Update update)
    {
        var chat = update.CallbackQuery?.Message?.Chat;
        
        if (chat is null)
            throw new NotSupportedException("Unable to extract chat id from CallbackQuery. TeleFlow requires callback queries with Message.Chat (inline mode is not supported).");

        return chat.Id;
    }


    private IServiceScope CreateScope(long chatId)
    {
        var scope = _serviceScopeFactory.CreateScope();
        
        var chatIdSetter = scope.ServiceProvider.GetRequiredService<IChatIdSetter>();
        chatIdSetter.SetChatId(chatId);

        return scope;
    }


    public UpdateReceiverMiddleware(IHandler<UpdateContext> next, IServiceScopeFactory scopeFactory)
    {
        _serviceScopeFactory = scopeFactory;
        Next = next;
    }
}