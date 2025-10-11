using Telegram.Bot.Extensions.Handlers.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers;

public class UpdateListener : IHandler<Update>, INavigatorHandler
{
    private ICommandHandler? _currentCommand;

    private readonly IHandlerFactoryWithArgs _handlerFactory;

    private readonly IHandlerFactoryWithArgs<ICommandHandler, Update, string> _stringHandlerFactory;

    public async Task Handle(Update args)
    {
        if(_currentCommand is not null)
        {
            await _currentCommand.Handle(args);
            return;
        }

        _handlerFactory.SetContext(args);

        var command = _handlerFactory.Create();
        
        await SetCurrentCommand(command);
    }

    public async Task Handle(string args)
    {
        _stringHandlerFactory.SetContext(args);
        var command = _stringHandlerFactory.Create();

        await SetCurrentCommand(command);
    }

    private async Task SetCurrentCommand(ICommandHandler command)
    {
        await OnCommandFinished();

        _currentCommand = command;
        _currentCommand.CommandFinished += OnCommandFinished;

        await _currentCommand.OnCommandCreated();
    }

    private async Task OnCommandFinished()
    {
        if(_currentCommand is not null)
            _currentCommand.CommandFinished -= OnCommandFinished;

        _currentCommand = null;
    }


    public UpdateListener(IHandlerFactoryWithArgs handlerFactory, IHandlerFactoryWithArgs<ICommandHandler, Update, string> stringHandlerFactory)
    {
        _handlerFactory = handlerFactory;
        _stringHandlerFactory = stringHandlerFactory;
    }

    internal UpdateListener(Func<INavigatorHandler, IHandlerFactoryWithArgs> commandFactory,
                            Func<INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> navigatorFactory)
    {
        _handlerFactory = commandFactory.Invoke(this);
        _stringHandlerFactory = navigatorFactory.Invoke(this);
    }
}