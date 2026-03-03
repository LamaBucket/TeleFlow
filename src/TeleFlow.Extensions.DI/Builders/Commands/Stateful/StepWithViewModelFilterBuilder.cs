using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.StepRegistration;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class StepWithViewModelFilterBuilder<TViewModel>
    where TViewModel : StepViewModel
{
    private readonly StepWithRenderRegistration<TViewModel> _rendererDescriptor;

    public void AddRendererDecorator(RenderDecorator<TViewModel> decorator)
    {
        _rendererDescriptor.AddRender(decorator);
    }

    public void AddFilter(Func<ICommandStepFilter> filter)
    {
        _rendererDescriptor.AddFilter(filter);
    }

    internal StepWithViewModelFilterBuilder(StepWithRenderRegistration<TViewModel> rendererDescriptor)
    {
        _rendererDescriptor = rendererDescriptor;
    }
}