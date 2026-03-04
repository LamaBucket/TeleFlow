using TeleFlow.Abstractions.Engine.Commands.Filters;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Descriptors;

public delegate ICommandStepFilter StepFilterFactory();

internal class StepFilterDescriptor
{
    public IReadOnlyList<StepFilterFactory> FilterFactories => _filterFactories;

    private readonly List<StepFilterFactory> _filterFactories = [];

    public void AddFilter(StepFilterFactory filterFactory)
        => _filterFactories.Add(filterFactory);
}