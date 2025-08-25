using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class UpdateListenerFactory : IHandlerFactoryWithArgs<UpdateListener, Update, IChatIdProvider>
{
    private readonly Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs> _handlerFactory;

    private readonly Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> _navigatorFactory;

    private IChatIdProvider? _chatIdProvider;

    public UpdateListener Create()
    {
        if(_chatIdProvider is null)
            throw new Exception("The Listener Does not know which chat it is in");


        var handlerFactory = (INavigatorHandler navHandler) =>
        {
            return _handlerFactory.Invoke(_chatIdProvider, navHandler);
        };

        var navigatorFactory = (INavigatorHandler navHandler) =>
        {
            return _navigatorFactory.Invoke(_chatIdProvider, navHandler);
        };

        var listener = new UpdateListener(handlerFactory, navigatorFactory);


        _chatIdProvider = null;

        return listener;
    }

    public void SetContext(IChatIdProvider args)
    {
        _chatIdProvider = args;
    }

    public UpdateListenerFactory(Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs> commandFactory, Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> navigatorFactory)
    {
        _handlerFactory = commandFactory;
        _navigatorFactory = navigatorFactory;
    }

    public UpdateListenerFactory(Func<IChatIdProvider, INavigatorHandler, UniversalCommandFactory> universalCommandFactory) : this(universalCommandFactory, universalCommandFactory)
    {
        
    }
}