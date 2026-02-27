using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Core.Commands.Decorators;

public class FilterCommandDecorator : ICommandHandler
{
    private readonly ICommandHandler _inner;

    private readonly ICommandFilter _filter;
    
    public Task<CommandResult> Handle(UpdateContext update)
        => _filter.Execute(update, _inner);

    public FilterCommandDecorator(ICommandHandler inner, ICommandFilter filter)
    {
        _inner = inner;
        _filter = filter;
    }
}