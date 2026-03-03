using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.StepRegistration;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class StepFilterBuilder
{
    protected readonly CommandStepRouterBuilder Builder;

    private readonly PlainStepRegistration _descriptor;


    public CommandStepRouterBuilder NextStep()
        => Builder;


    public StepFilterBuilder AddFilter(Func<ICommandStepFilter> filter)
    {
        _descriptor.AddFilter(filter);

        return this;
    }

    internal StepFilterBuilder(PlainStepRegistration descriptor, CommandStepRouterBuilder builder)
    {
        _descriptor = descriptor;
        Builder = builder;
    }
}