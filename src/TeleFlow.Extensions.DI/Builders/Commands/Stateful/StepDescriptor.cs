using TeleFlow.Abstractions.Engine.Commands.Interceptors;
using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

internal class StepDescriptor
{
    public Func<ICommandStep> StepFactory { get; init; }

    public IReadOnlyList<Func<ICommandStepInterceptor>> Interceptors => _interceptors;

    private readonly List<Func<ICommandStepInterceptor>> _interceptors = [];


    public void AddInterceptor(Func<ICommandStepInterceptor> interceptor)
        => _interceptors.Add(interceptor);


    public StepDescriptor(Func<ICommandStep> stepFactory)
    {
        StepFactory = stepFactory;
    }
}