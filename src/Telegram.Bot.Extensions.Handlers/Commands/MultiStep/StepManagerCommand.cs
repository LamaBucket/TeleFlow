using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
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
            throw new Exception("Step Manager is intended to only moderate the current chain!");

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
