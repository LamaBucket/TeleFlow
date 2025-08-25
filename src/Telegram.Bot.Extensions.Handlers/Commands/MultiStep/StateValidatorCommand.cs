using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Models.StepCommands;
using LisBot.Common.Telegram.Services;
using LisBot.Common.Telegram.ViewModels;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram.Commands.MultiStep;

public class StateValidatorCommand<TState> : ICommandHandler
{
    public event Func<Task>? CommandFinished;

    protected ICommandHandler Next => _next ?? throw new Exception("Intended to work only with state");

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
            if(_next is null)
                throw new Exception("Next Should be newer null in here!");

            await _next.Handle(args);
        }
    }

    private async Task FinalizeCommand()
    {
        if(_next is null)
            throw new Exception("Cannot Finalize Current command");

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
