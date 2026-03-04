using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Descriptors;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Registrations;

public delegate IStepRenderService<TData> RenderServiceProvider<TData>() where TData : StepData;

public delegate StatefulStep<TData> StepWithRenderPipelineFactory<TData>(IStepRenderService<TData> renderService) where TData : StepData;


internal class StepWithRenderPipelineRegistration<TData> : IStepRegistration
    where TData : StepData
{
    private readonly RenderServiceProvider<TData> _renderServiceProvider;
    
    private readonly StepWithRenderPipelineFactory<TData> _factory;


    private readonly StepFilterDescriptor _filterDescriptor;

    private readonly StepRenderPipelineDescriptor<TData> _renderServiceDescriptor;


    public CommandStepFactory CompileStepFactory()
        => () =>
        {
            var renderService = _renderServiceProvider();

            foreach(var renderServiceFactory in _renderServiceDescriptor.RenderServiceFactories)
            {
                renderService = renderServiceFactory(renderService);
            }


            ICommandStep step = _factory.Invoke(renderService);

            foreach(var filterFactory in _filterDescriptor.FilterFactories)
            {
                var filter = filterFactory();

                step = new FilterCommandStepDecorator(step, filter);
            };

            return step;
        };

    public StepWithRenderPipelineRegistration(RenderServiceProvider<TData> baseRenderService, StepWithRenderPipelineFactory<TData> factory, StepFilterDescriptor filterDescriptor, StepRenderPipelineDescriptor<TData> renderServiceDescriptor)
    {
        _renderServiceProvider = baseRenderService;
        _factory = factory;
        _filterDescriptor = filterDescriptor;
        _renderServiceDescriptor = renderServiceDescriptor;
    }
}