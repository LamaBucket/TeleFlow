using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Descriptors;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;

public class StepFilterBuilder
{
    private protected readonly StepFilterDescriptor FilterDescriptor;

    public StepFilterBuilder AddFilter(StepFilterFactory filterFactory)
    {
        FilterDescriptor.AddFilter(filterFactory);

        return this;
    }

    internal StepFilterBuilder(StepFilterDescriptor filterDescriptor)
    {
        FilterDescriptor = filterDescriptor;
    }
}