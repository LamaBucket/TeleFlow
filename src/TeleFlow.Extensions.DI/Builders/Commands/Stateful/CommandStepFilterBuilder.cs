using TeleFlow.Abstractions.Engine.Commands.Filters;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class CommandStepFilterBuilder
{
    private readonly StepDescriptor _descriptor;

    private readonly CommandStepRouterBuilder _builder;


    public CommandStepRouterBuilder NextStep()
        => _builder;


    public CommandStepFilterBuilder AddFilter(Func<ICommandStepFilter> filter)
    {
        _descriptor.AddFilter(filter);

        return this;
    }

    internal CommandStepFilterBuilder(StepDescriptor descriptor, CommandStepRouterBuilder builder)
    {
        _descriptor = descriptor;
        _builder = builder;
    }
}