using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram;

public class UpdateDistributor : IHandler<Update>
{
    private readonly Dictionary<long, IHandler<Update>> _chatHandlers;


    private readonly IHandlerFactoryWithArgs<UpdateListener, Update, IChatIdProvider> _updateListenerFactory;

    private readonly Func<UpdateListener, IHandler<Update>> _updateListenerPostCreationSetup;


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
            var updateListener = GenerateUpdateListener(chatIdProvider);

            var handler = _updateListenerPostCreationSetup.Invoke(updateListener);

            _chatHandlers.Add(chatId, handler);
        }

        return _chatHandlers[chatId];
    }

    private UpdateListener GenerateUpdateListener(IChatIdProvider chatIdProvider)
    {
        _updateListenerFactory.SetContext(chatIdProvider);
        return _updateListenerFactory.Create();
    }

    public UpdateDistributor(IHandlerFactoryWithArgs<UpdateListener, Update, IChatIdProvider> userHandlerFactory, Func<UpdateListener, IHandler<Update>> updateListenerPostCreationSetup)
    {
        _chatHandlers = [];
        _updateListenerFactory = userHandlerFactory;
        _updateListenerPostCreationSetup = updateListenerPostCreationSetup;
    }
}