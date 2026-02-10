using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Pipeline.Middlewares;

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
        return args.GetChatId();
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