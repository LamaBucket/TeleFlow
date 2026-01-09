using TeleFlow.Commands;
using TeleFlow.Commands.Statefull;

namespace TeleFlow.Factories;

public class StepCommandFactory
{
    public int StepCount => _stepFactories.Count;

    private readonly List<Func<IStepCommand>> _stepFactories;

    private bool _usedInCreation;

    public IStepCommand Create(int context)
    {
        if(context < 0)
            throw new Exception("Step Number Cannot be less than 0!");

        if(context >= StepCount)
            throw new Exception("Step Number is Higher than step factory count!");

        var stepFactory = _stepFactories[context];

        var factory = stepFactory.Invoke();

        _usedInCreation = true;

        return factory;
    }

    public void AddStepFactory(Func<IStepCommand> factory, bool prepend = false)
    {
        if(_usedInCreation)
            throw new Exception("This Factory was already used in creating StepCommands!");

        _stepFactories.Insert(prepend ? 0 : _stepFactories.Count, factory);
    }

    public StepCommandFactory()
    {
        _stepFactories = [];
        _usedInCreation = false;
    }
}