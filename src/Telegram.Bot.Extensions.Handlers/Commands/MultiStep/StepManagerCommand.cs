using LisBot.Common.Telegram.Exceptions;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep;

public class StepManagerCommand : ICommandHandler
{
    public event Func<Task>? CommandFinished;


    private StepCommand? _next => _stepChainBuilder.Head;

    private readonly StepChainBuilder _stepChainBuilder;


    public async Task Handle(Update args)
    {
        if(_next is null)
            throw new UnableToHandleException(nameof(StepManagerCommand), "Next was null. Step Manager does not handle updates itself.");

        await _next.Handle(args);
    }


    public async Task OnCommandCreated()
    {
        var lastChainItem = _stepChainBuilder.BuildChain();

        await lastChainItem.OnCommandCreated();
    }


    private async Task OnChainFinished()
    {
        if(CommandFinished is not null)
            await CommandFinished.Invoke();
    }

    public StepManagerCommand(StepChainBuilder stepChainBuilder)
    {
        _stepChainBuilder = stepChainBuilder;
        _stepChainBuilder.ChainFinished += OnChainFinished;
    }
}
