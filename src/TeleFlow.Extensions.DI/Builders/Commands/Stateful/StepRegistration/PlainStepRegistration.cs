using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Core.Commands.Decorators;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.StepRegistration;

internal class PlainStepRegistration : IStepRegistration
{
    private readonly Func<ICommandStep> _stepFactory;

    private readonly List<Func<ICommandStepFilter>> _filters = [];


    public void AddFilter(Func<ICommandStepFilter> filter)
        => _filters.Add(filter);


    public Func<ICommandStep> CompileFactory()
    {
        if(_filters.Count == 0)
            return _stepFactory;
        
        return () =>
        {
            ICommandStep step = _stepFactory();

            foreach(var filterFactory in _filters)
            {
                var filter = filterFactory.Invoke();

                step = new FilterCommandStepDecorator(step, filter);
            }

            return step;
        };
    }

    public PlainStepRegistration(Func<ICommandStep> stepFactory)
    {
        _stepFactory = stepFactory;
    }
}