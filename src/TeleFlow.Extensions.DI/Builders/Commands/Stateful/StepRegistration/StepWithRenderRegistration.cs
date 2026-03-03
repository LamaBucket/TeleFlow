using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.StepRegistration;

public delegate IStepRenderService<TViewModel> RenderDecorator<TViewModel>(IStepRenderService<TViewModel> inner)
    where TViewModel : StepViewModel;

internal class StepWithRenderRegistration<TViewModel> : IStepRegistration
    where TViewModel : StepViewModel
{
    private readonly Func<IStepRenderService<TViewModel>, ICommandStep> _stepFactory;

    private readonly Func<IStepRenderService<TViewModel>> _renderFactory;


    private readonly List<Func<ICommandStepFilter>> _filters = [];

    private readonly List<RenderDecorator<TViewModel>> _renders = [];


    public void AddFilter(Func<ICommandStepFilter> filter)
        => _filters.Add(filter);

    public void AddRender(RenderDecorator<TViewModel> render)
        => _renders.Add(render);
        

    public Func<ICommandStep> CompileFactory()
        => ()  => {

            var renderService = _renderFactory();

            foreach(var renderDecorator in _renders)
            {
                renderService = renderDecorator(renderService);
            }

            var step = _stepFactory(renderService);

            foreach(var filterFactory in _filters)
            {
                var filter = filterFactory.Invoke();

                step = new FilterCommandStepDecorator(step, filter);
            }
            
            return step;
        };
    

    public StepWithRenderRegistration(Func<IStepRenderService<TViewModel>, ICommandStep> stepFactory, Func<IStepRenderService<TViewModel>> renderFactory)
    {
        _stepFactory = stepFactory;
        _renderFactory = renderFactory;
    }
}