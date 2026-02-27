using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Core.Commands.Decorators;

public class FilterCommandStepDecorator : ICommandStep
{
    private readonly ICommandStep _inner;

    private readonly ICommandStepFilter _filter;
    
    public Task<CommandStepResult> Handle(UpdateContext args)
        => _filter.ExecuteOnUpdate(args, _inner.Handle);

    public Task OnEnter(IServiceProvider serviceProvider)
        => _filter.ExecuteOnEnter(serviceProvider, _inner.OnEnter);


    public FilterCommandStepDecorator(ICommandStep inner, ICommandStepFilter filter)
    {
        _inner = inner;
        _filter = filter;
    }
}