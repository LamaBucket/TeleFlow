using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Core.Commands.Stateful;

public class CommandStepRouter
{
    public int StepCount => _stepFactories.Length;

    private readonly Func<ICommandStep>[] _stepFactories;

    public ICommandStep Create(int context)
    {
        if(context < 0)
            throw new Exception("Step Number Cannot be less than 0!");

        if(context >= StepCount)
            throw new Exception("Step Number is Higher than step factory count!");

        var stepFactory = _stepFactories[context];

        return stepFactory.Invoke();
    }

    public CommandStepRouter(Func<ICommandStep>[] stepFactories)
    {
        _stepFactories = stepFactories;
    }
}