using TeleFlow.Commands;
using TeleFlow.Commands.Statefull;

namespace TeleFlow.Factories;

public class StepCommandFactory
{
    public int StepCount => _stepFactories.Length;

    private readonly Func<IFlowStep>[] _stepFactories;

    public IFlowStep Create(int context)
    {
        if(context < 0)
            throw new Exception("Step Number Cannot be less than 0!");

        if(context >= StepCount)
            throw new Exception("Step Number is Higher than step factory count!");

        var stepFactory = _stepFactories[context];

        return stepFactory.Invoke();
    }

    public StepCommandFactory(Func<IFlowStep>[] stepFactories)
    {
        _stepFactories = stepFactories;
    }
}