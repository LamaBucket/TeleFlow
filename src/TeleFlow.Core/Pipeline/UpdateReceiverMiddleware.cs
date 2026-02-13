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

        using(var scope = CreateScope(chatId))
        {
            UpdateContext context = new(args, scope.ServiceProvider);
            await (Next?.Handle(context) ?? throw new InvalidOperationException());
        }
    }


    protected virtual long GetChatId(Update args)
    {
        return GetChatIdDefault(args);
    }

    private static long GetChatIdDefault(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id ?? throw new NullReferenceException("Unable To Retrieve Chat Id From Message"),
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id ?? throw new NullReferenceException("Unable To Retrieve Chat Id From CallbackQuery"),
            _ => throw new InvalidOperationException("This MessageType is not supported"),
        };
    }

    private IServiceScope CreateScope(long chatId)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var chatIdSetter = scope.ServiceProvider.GetService<IChatIdSetter>() ?? throw new InvalidOperationException();
        chatIdSetter.SetChatId(chatId);

        return scope;
    }


    public UpdateReceiverMiddleware(IHandler<UpdateContext> next, IServiceScopeFactory scopeFactory)
    {
        _serviceScopeFactory = scopeFactory;
        Next = next;
    }
}