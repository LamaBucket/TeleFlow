using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Results;

namespace TeleFlow.Extensions.DI.Builders.Commands;

public class CommandFilterBuilder
{
    private readonly CommandRouterBuilder _builder;

    private readonly CommandDescriptor _descriptor;

    private readonly Action _ensureNotBuilt;


    public CommandRouterBuilder NextCommand()
        => _builder;


    public CommandFilterBuilder EnableNavigation(Func<NavigateCommandParameters, IServiceProvider, Task>? seed = null)
    {
        _ensureNotBuilt();
        _descriptor.EnableNavigation(seed);
        return this;
    }

    public CommandFilterBuilder AddFilter(Func<ICommandFilter> filter)
    {
        _ensureNotBuilt();
        _descriptor.AddFilter(filter);
        return this;
    }


    internal CommandFilterBuilder(CommandRouterBuilder builder, CommandDescriptor descriptor, Action ensureNotBuilt)
    {
        _builder = builder;
        _descriptor = descriptor;
        _ensureNotBuilt = ensureNotBuilt;
    }
}