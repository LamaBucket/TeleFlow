using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Core.Commands.Stateful;

public class CommandStepRouter
{
    public int StepCount => _stepFactories.Length;

    private readonly CommandStepFactory[] _stepFactories;

    public ICommandStep Create(int context)
    {
        var stepIndex = context;
        
        ArgumentOutOfRangeException.ThrowIfNegative(stepIndex, nameof(stepIndex));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(stepIndex, StepCount, nameof(stepIndex));
        
        var stepFactory = _stepFactories[context];

        return stepFactory.Invoke();
    }

    public CommandStepRouter(CommandStepFactory[] stepFactories)
    {
        _stepFactories = stepFactories;
    }
}