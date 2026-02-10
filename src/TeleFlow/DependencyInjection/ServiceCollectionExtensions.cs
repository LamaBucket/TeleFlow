using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Bootstrap;

namespace TeleFlow.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddTeleFlow(this IServiceCollection services, Action<TeleFlowConfiguration> options)
    {
        TeleFlowDefaultServicesInternal.ConfigureServices(services);

        var configuration = TeleFlowConfiguration.Default;

        options.Invoke(configuration);

        TeleFlowDefaultServicesInternal.ConfigureMiddlewarePipeline(services, configuration.MiddlewareConfiguration);

        TeleFlowDefaultServicesInternal.ConfigureCommandRegistries(services, configuration.CommandRegistryConfiguration);
    }
}