using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class StateValidatorCommandFactory<TState> : ICommandFactory<ICommandHandler, Update, ICommandHandler>
{
    private ICommandHandler? _next;


    private readonly StepChainBuilder _stepChainBuilder;

    private readonly StateValidationMessageFormatter<TState> _messageFormatter;

    private readonly IMessageService<Message> _messageService;

    private readonly State<TState> _state;


    public ICommandHandler Create()
    {
        if(_next is null)
            throw new Exception("The Next was null");

        return new StateValidatorCommand<TState>(_next, _stepChainBuilder, _messageFormatter, _messageService, _state);
    }

    public void SetContext(ICommandHandler args)
    {
        _next = args;
    }

    public StateValidatorCommandFactory(StepChainBuilder stepChainBuilder, StateValidationMessageFormatter<TState> messageFormatter, IMessageService<Message> messageService, State<TState> state)
    {
        _stepChainBuilder = stepChainBuilder;
        _messageFormatter = messageFormatter;
        _messageService = messageService;
        _state = state;
    }
}
