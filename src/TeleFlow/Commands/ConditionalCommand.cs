using Telegram.Bot.Types;

namespace TeleFlow.Commands;

public class ConditionalCommand : ICommandHandler
{
    private ICommandHandler? _nextHandler;


    private readonly ICommandHandler _handlerIfMeetsCondition;

    private readonly ICommandHandler _handlerIfNotMeetsCondition;


    private readonly Func<Task<bool>> _condition;
    

    public event Func<Task>? CommandFinished;


    public async Task Handle(Update args)
    {
        if(_nextHandler is null)
            throw new NullReferenceException(nameof(_nextHandler));
            
        await _nextHandler.Handle(args);
    }

    public async Task OnCommandCreated()
    {
        bool conditionResult = await _condition();

        if(conditionResult)
            _nextHandler = _handlerIfMeetsCondition;
        else
            _nextHandler = _handlerIfNotMeetsCondition;
        
        _nextHandler.CommandFinished += OnNextFinished;
        await _nextHandler.OnCommandCreated();
    }

    private async Task OnNextFinished()
    {
        if(_nextHandler is null)
            throw new NullReferenceException(nameof(_nextHandler));
            
        _nextHandler.CommandFinished -= OnNextFinished;
        _handlerIfMeetsCondition.CommandFinished -= OnNextFinished;
        _handlerIfNotMeetsCondition.CommandFinished -= OnNextFinished;
        
        if(CommandFinished is not null)
            await CommandFinished.Invoke();
    }

    public ConditionalCommand(Func<Task<bool>> condition, ICommandHandler handlerIfMeetsCondition, ICommandHandler handlerIfNotMeetsCondition)
    {
        _condition = condition;
        _handlerIfMeetsCondition = handlerIfMeetsCondition;
        _handlerIfNotMeetsCondition = handlerIfNotMeetsCondition;
    }
}
