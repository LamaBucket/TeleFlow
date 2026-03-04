using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Descriptors;

public delegate IStepRenderService<TData> StepRenderPostProcessor<TData>(IStepRenderService<TData> baseRenderService)
    where TData : StepData;

internal class StepRenderPipelineDescriptor<TData>
    where TData : StepData
{
    public IReadOnlyList<StepRenderPostProcessor<TData>> RenderServiceFactories => _renderServiceFactories;

    private readonly List<StepRenderPostProcessor<TData>> _renderServiceFactories = [];

    public void AddRenderPostProcessor(StepRenderPostProcessor<TData> renderPostProcessorFactory)
        => _renderServiceFactories.Add(renderPostProcessorFactory);
}