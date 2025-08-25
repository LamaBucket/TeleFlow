using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class UpdateListenerFactory : IHandlerFactoryWithArgs<UpdateListener, Update, IChatIdProvider>
{
    private readonly Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs> _processorFactory;

    private readonly Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> _navigatorFactory;

    private IChatIdProvider? _chatIdProvider;

    public UpdateListener Create()
    {
        if(_chatIdProvider is null)
            throw new Exception("The Listener Does not know which chat it is in");

        var listener = new UpdateListener(_processorFactory, _navigatorFactory, _chatIdProvider);

        _chatIdProvider = null;

        return listener;
    }

    public void SetContext(IChatIdProvider args)
    {
        _chatIdProvider = args;
    }

    public UpdateListenerFactory(Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs> commandFactory, Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> navigatorFactory)
    {
        _processorFactory = commandFactory;
        _navigatorFactory = navigatorFactory;
    }

    public UpdateListenerFactory(Func<IChatIdProvider, INavigatorHandler, UniversalCommandFactory> universalCommandFactory) : this(universalCommandFactory, universalCommandFactory)
    {
        
    }
}