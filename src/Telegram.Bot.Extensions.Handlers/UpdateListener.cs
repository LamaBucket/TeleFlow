using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram;

public class UpdateListener : IHandler<Update>, INavigatorHandler
{
    private ICommandHandler? _currentCommand;

    private readonly IHandlerFactoryWithArgs _handlerFactory;

    private readonly IHandlerFactoryWithArgs<ICommandHandler, Update, string> _stringHandlerFactory;

    private readonly IChatIdProvider _chatIdProvider;

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


    public UpdateListener(IHandlerFactoryWithArgs handlerFactory, IHandlerFactoryWithArgs<ICommandHandler, Update, string> stringHandlerFactory, IChatIdProvider chatIdProvider)
    {
        _handlerFactory = handlerFactory;
        _stringHandlerFactory = stringHandlerFactory;
        _chatIdProvider = chatIdProvider;
    }

    internal UpdateListener(Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs> commandFactory,
                            Func<IChatIdProvider, INavigatorHandler, IHandlerFactoryWithArgs<ICommandHandler, Update, string>> navigatorFactory,
                            IChatIdProvider chatIdProvider)
    {
        _handlerFactory = commandFactory.Invoke(chatIdProvider, this);
        _stringHandlerFactory = navigatorFactory.Invoke(chatIdProvider, this);
        _chatIdProvider = chatIdProvider;
    }
}