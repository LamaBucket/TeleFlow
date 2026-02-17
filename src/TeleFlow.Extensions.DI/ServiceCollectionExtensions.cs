using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Extensions.DI.Configuration;
using TeleFlow.Extensions.DI.Configuration.Default;

namespace TeleFlow.Extensions.DI;

public static class ServiceCollectionExtensions
{
    public static void AddTeleFlowServices(this IServiceCollection services, Action<TeleFlowConfiguration> options)
    {
        TeleFlowDefaultServicesInternal.ConfigureServices(services);

        var configuration = TeleFlowConfiguration.Default;

        options.Invoke(configuration);

        TeleFlowDefaultServicesInternal.ConfigureMiddlewarePipeline(services, configuration.MiddlewareConfiguration);

        TeleFlowDefaultServicesInternal.ConfigureCommandRegistries(services, configuration.CommandRegistryConfiguration);
    }
}