using TeleFlow.Factories.CommandFactories;
using TeleFlow.Models;
using Telegram.Bot.Types;

namespace TeleFlow.Factories;

public class UpdateListenerFactory<TBuildArgs> : IHandlerFactoryWithArgs<UpdateListener, Update, TBuildArgs> where TBuildArgs : class
{
    private readonly Func<TBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs> _handlerFactory;

    private readonly Func<TBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> _navigatorFactory;

    private TBuildArgs? _args;

    public UpdateListener Create()
    {
        if(_args is null)
            throw new ArgumentNullException(nameof(_args));


        var handlerFactory = (INavigatorHandler navHandler) =>
        {
            return _handlerFactory.Invoke(_args, navHandler);
        };

        var navigatorFactory = (INavigatorHandler navHandler) =>
        {
            return _navigatorFactory.Invoke(_args, navHandler);
        };

        var listener = new UpdateListener(handlerFactory, navigatorFactory);


        _args = null;

        return listener;
    }

    public void SetContext(TBuildArgs args)
    {
        _args = args;
    }

    public UpdateListenerFactory(Func<TBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs> commandFactory, Func<TBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> navigatorFactory)
    {
        _handlerFactory = commandFactory;
        _navigatorFactory = navigatorFactory;
    }

    public UpdateListenerFactory(Func<TBuildArgs, INavigatorHandler, UniversalCommandFactory> universalCommandFactory) : this(universalCommandFactory, universalCommandFactory)
    {
        
    }
}