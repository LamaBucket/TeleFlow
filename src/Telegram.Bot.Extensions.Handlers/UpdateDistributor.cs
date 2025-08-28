using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram;

public class UpdateDistributor : IHandler<Update>
{
    private readonly Dictionary<long, IHandler<Update>> _chatHandlers;

    private readonly IHandlerFactoryWithArgs<IHandler<Update>, Update, IChatIdProvider> _nextHandlerFactory;


    public async Task Handle(Update args)
    {
        await GetNextHandler(args).Handle(args);
    }

    private IHandler<Update> GetNextHandler(Update args)
    {
        var chatIdProvider = new FromUpdateChatIdProvider(args);
        var chatId = chatIdProvider.GetChatId();

        if(!_chatHandlers.ContainsKey(chatId))
        {   
            var handler = GenerateNextHandler(chatIdProvider);

            _chatHandlers.Add(chatId, handler);
        }

        return _chatHandlers[chatId];
    }

    private IHandler<Update> GenerateNextHandler(IChatIdProvider chatIdProvider)
    {
        _nextHandlerFactory.SetContext(chatIdProvider);
        return _nextHandlerFactory.Create();
    }

    public UpdateDistributor(IHandlerFactoryWithArgs<IHandler<Update>, Update, IChatIdProvider> nextHandlerFactory)
    {
        _chatHandlers = [];
        _nextHandlerFactory = nextHandlerFactory;
    }
}