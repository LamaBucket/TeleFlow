using TeleFlow.Abstractions.State.Step;
using TeleFlow.Fluent.Builders.Commands.Stateful.Filters.Descriptors;

namespace TeleFlow.Fluent.Builders.Commands.Stateful.Filters;

public class StepWithRenderPipelineFilterBuilder<TData> : StepFilterBuilder
    where TData : StepData
{
    private readonly StepRenderPipelineDescriptor<TData> _renderServiceDescriptor;

    public StepWithRenderPipelineFilterBuilder<TData> AddRenderService(StepRenderPostProcessor<TData> renderServiceFactory)
    {
        _renderServiceDescriptor.AddRenderPostProcessor(renderServiceFactory);
        return this;
    }

    internal StepWithRenderPipelineFilterBuilder(StepFilterDescriptor filterDescriptor, StepRenderPipelineDescriptor<TData> renderServiceDescriptor) : base(filterDescriptor)
    {
        _renderServiceDescriptor = renderServiceDescriptor;
    }
}