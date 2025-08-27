using LisBot.Common.Telegram.Exceptions;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep;

public class StateValidatorCommand<TState> : ICommandHandler
{
    public event Func<Task>? CommandFinished;

    protected ICommandHandler Next => _next ?? throw new UnableToHandleException(nameof(StateValidatorCommand<TState>), "Next was null. State Validator does not handle updates itself.");

    private ICommandHandler? _next;

    private readonly StepChainBuilder _stepChainBuilder;


    private readonly StateValidationMessageFormatter<TState> _messageFormatter;

    private readonly IMessageService<Message> _messageService;


    private readonly State<TState> _state;

    private bool _awaitsUserInput;

    public async Task Handle(Update args)
    {
        if(_awaitsUserInput)
        {
            var commandToRestart = _messageFormatter.ParseUserResponse(args);
            
            _awaitsUserInput = false;

            if(commandToRestart is null)
                await FinalizeCommand();
            else
                await RestartStep(commandToRestart);
        }
        else
        {
            await Next.Handle(args);
        }
    }

    private async Task FinalizeCommand()
    {
        if(_next is null)
            throw new NullReferenceException(nameof(_next));

        _next.CommandFinished -= OnNextFinished;
        _next = null;

        if(CommandFinished is not null)
            await CommandFinished.Invoke();
    }

    private async Task RestartStep(IHandlerFactoryWithArgs<StepCommand, Update, StepCommand> commandToRecreate)
    {
        var lastChainItem = _stepChainBuilder.BuildChain(commandToRecreate);
        
        await lastChainItem.OnCommandCreated();
    }


    public async Task OnCommandCreated()
    {
        await Next.OnCommandCreated();
    }


    public async Task OnNextFinished()
    {
        var msg = _messageFormatter.GenerateMessage(_state.CurrentValue);

        await _messageService.SendMessage(msg);

        _awaitsUserInput = true;
    }


    public StateValidatorCommand(ICommandHandler next, StepChainBuilder stepChainBuilder, StateValidationMessageFormatter<TState> messageFormatter, IMessageService<Message> messageService, State<TState> state)
    {
        _next = next;
        _stepChainBuilder = stepChainBuilder;

        _messageFormatter = messageFormatter;
        _messageService = messageService;


        _state = state;
        
        Next.CommandFinished += OnNextFinished;

        _awaitsUserInput = false;
    }
}
