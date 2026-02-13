using TeleFlow.Abstractions.Engine.Commands.Interceptors;
using TeleFlow.Abstractions.Engine.Commands.Results;

namespace TeleFlow.Extensions.DependencyInjection.Builders.Commands;

public class CommandInterceptorBuilder
{
    private readonly CommandDescriptor _descriptor;

    private readonly Action _ensureNotBuilt;


    public CommandInterceptorBuilder EnableNavigation(Func<NavigateCommandParameters, IServiceProvider, Task>? seed = null)
    {
        _ensureNotBuilt();
        _descriptor.EnableNavigation(seed);
        return this;
    }

    public CommandInterceptorBuilder AddInterceptor(Func<ICommandInterceptor> interceptorFactory)
    {
        _ensureNotBuilt();
        _descriptor.AddInterceptor(interceptorFactory);
        return this;
    }


    internal CommandInterceptorBuilder(CommandDescriptor descriptor, Action ensureNotBuilt)
    {
        _descriptor = descriptor;
        _ensureNotBuilt = ensureNotBuilt;
    }
}