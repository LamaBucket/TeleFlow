using TeleFlow.Abstractions.Engine.Commands.Interceptors;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class StepInterceptorBuilder
{
    private readonly StepDescriptor _descriptor;

    private readonly CommandStepRouterBuilder _builder;


    public CommandStepRouterBuilder NextStep()
        => _builder;


    public StepInterceptorBuilder AddInterceptor(Func<ICommandStepInterceptor> interceptor)
    {
        _descriptor.AddInterceptor(interceptor);

        return this;
    }

    internal StepInterceptorBuilder(StepDescriptor descriptor, CommandStepRouterBuilder builder)
    {
        _descriptor = descriptor;
        _builder = builder;
    }
}