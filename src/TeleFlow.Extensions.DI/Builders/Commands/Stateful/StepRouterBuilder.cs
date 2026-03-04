using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Descriptors;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Registrations;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class StepRouterBuilder
{
    private readonly List<IStepRegistration> _registrations;

    public StepWithRenderPipelineFilterBuilder<TData> Add<TData>(RenderServiceProvider<TData> baseRenderService, StepWithRenderPipelineFactory<TData> factory, bool prepend = false) 
        where TData : StepData
    {
        StepFilterDescriptor filterDescriptor = new();
        StepRenderPipelineDescriptor<TData> renderServiceDescriptor = new();

        StepWithRenderPipelineRegistration<TData> registration = new(baseRenderService, factory, filterDescriptor, renderServiceDescriptor);        
        _registrations.Insert(prepend ? 0 : _registrations.Count, registration);

        return new(filterDescriptor, renderServiceDescriptor);
    }


    public StepFilterBuilder Add(CommandStepFactory factory, bool prepend = false)
    {
        StepFilterDescriptor filterDescriptor = new();

        StepRegistration registration = new(factory, filterDescriptor);

        _registrations.Insert(prepend ? 0 : _registrations.Count, registration);

        return new(filterDescriptor);
    }

    public CommandStepRouter Build()
    {
        if(_registrations.Count == 0)
            throw new InvalidOperationException("Cannot build CommandStepRouter: no steps were configured.");

        CommandStepFactory[] stepFactories = new CommandStepFactory[_registrations.Count];

        for(int i = 0; i < _registrations.Count; i++)
        {
            var stepRegistration = _registrations[i];
            var stepFactory = stepRegistration.CompileStepFactory();

            stepFactories[i] = stepFactory.Invoke;
        }

        return new CommandStepRouter(stepFactories);
    }

    public StepRouterBuilder()
    {
        _registrations = [];
    }
}