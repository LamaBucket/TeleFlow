using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

internal class StepDescriptor
{
    public Func<ICommandStep> StepFactory { get; init; }

    public IReadOnlyList<Func<ICommandStepFilter>> Filters => _filters;

    private readonly List<Func<ICommandStepFilter>> _filters = [];


    public void AddFilter(Func<ICommandStepFilter> filter)
        => _filters.Add(filter);


    public StepDescriptor(Func<ICommandStep> stepFactory)
    {
        StepFactory = stepFactory;
    }
}