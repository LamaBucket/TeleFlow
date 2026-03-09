using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Fluent.Builders.Commands.Stateful.Filters.Descriptors;

namespace TeleFlow.Fluent.Builders.Commands.Stateful.Filters.Registrations;

internal class StepRegistration : IStepRegistration
{
    private readonly CommandStepFactory _factory;

    private readonly StepFilterDescriptor _filterDescriptor;

    public CommandStepFactory CompileStepFactory()
        => () =>
        {
            var step = _factory.Invoke();

            foreach(var filterFactory in _filterDescriptor.FilterFactories)
            {
                var filter = filterFactory();

                step = new FilterCommandStepDecorator(step, filter);
            };

            return step;
        };

    public StepRegistration(CommandStepFactory factory, StepFilterDescriptor filterDescriptor)
    {
        _factory = factory;
        _filterDescriptor = filterDescriptor;
    }
}