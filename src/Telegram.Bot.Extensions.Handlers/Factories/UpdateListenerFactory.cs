using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class UpdateListenerFactory : IHandlerFactoryWithArgs<IHandler<Update>, Update, UpdateDistributorNextHandlerBuildArgs>
{
    private readonly Func<UpdateDistributorNextHandlerBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs> _handlerFactory;

    private readonly Func<UpdateDistributorNextHandlerBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> _navigatorFactory;

    private UpdateDistributorNextHandlerBuildArgs? _args;

    public IHandler<Update> Create()
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

    public void SetContext(UpdateDistributorNextHandlerBuildArgs args)
    {
        _args = args;
    }

    public UpdateListenerFactory(Func<UpdateDistributorNextHandlerBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs> commandFactory, Func<UpdateDistributorNextHandlerBuildArgs, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> navigatorFactory)
    {
        _handlerFactory = commandFactory;
        _navigatorFactory = navigatorFactory;
    }

    public UpdateListenerFactory(Func<UpdateDistributorNextHandlerBuildArgs, INavigatorHandler, UniversalCommandFactory> universalCommandFactory) : this(universalCommandFactory, universalCommandFactory)
    {
        
    }
}