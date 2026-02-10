using TeleFlow.Commands;
using TeleFlow.Factories;
using TeleFlow.Interceptors;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Services;

namespace TeleFlow.Builders;

public class CommandInterceptorsBuilder
{
    private readonly CommandDescriptor _descriptor;

    private readonly Action _ensureNotBuilt;


    public CommandInterceptorsBuilder EnableNavigation(Func<NavigateCommandParameters, IServiceProvider, Task>? seed = null)
    {
        _ensureNotBuilt();
        _descriptor.EnableNavigation(seed);
        return this;
    }

    public CommandInterceptorsBuilder AddInterceptor(Func<ICommandInterceptor> interceptorFactory)
    {
        _ensureNotBuilt();
        _descriptor.AddInterceptor(interceptorFactory);
        return this;
    }


    internal CommandInterceptorsBuilder(CommandDescriptor descriptor, Action ensureNotBuilt)
    {
        _descriptor = descriptor;
        _ensureNotBuilt = ensureNotBuilt;
    }
}