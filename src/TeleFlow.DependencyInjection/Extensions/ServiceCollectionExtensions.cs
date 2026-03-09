using Microsoft.Extensions.DependencyInjection;
using TeleFlow.DependencyInjection.Extensions.Internal;

namespace TeleFlow.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTeleFlowServices(this IServiceCollection services)
        => TeleFlowDefaultServicesInternal.ConfigureServices(services);
}