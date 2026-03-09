using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.DependencyInjection.Extensions;
using TeleFlow.Fluent.Configuration;
using TeleFlow.Fluent.Extensions;
using TeleFlow.Hosting.Polling.Configuration;
using TeleFlow.Hosting.Polling.Services;
using Telegram.Bot;

namespace TeleFlow.Hosting.Polling;

public static class ServiceCollectionExtensions
{
    public static void AddTeleFlowPolling(this IServiceCollection services, string botToken)
    {        
        static void noActionsTeleFlowSetup(TeleFlowConfiguration options) { }
        
        services.AddTeleFlowPolling(botToken, noActionsTeleFlowSetup);
    }

    public static void AddTeleFlowPolling(this IServiceCollection services, string botToken, Action<TeleFlowConfiguration> configure)
    {
        TeleFlowPollingConfiguration polling = new(){ BotToken = botToken };
        services.AddTeleFlowPolling(polling, configure);
    }

    public static void AddTeleFlowPolling(this IServiceCollection services, TeleFlowPollingConfiguration polling, Action<TeleFlowConfiguration> configure)
    {
        services.AddTeleFlowServices();
        services.AddTeleFlowPipeline(configure);
        services.TryAddTeleFlowTelegramClient(polling);

        services.TryAddSingleton<TeleFlowPollingConfiguration>();

        services.AddHostedService<TeleFlowPollingHostedService>();
    }


    private static IServiceCollection TryAddTeleFlowTelegramClient(this IServiceCollection services, TeleFlowPollingConfiguration options)
    {
        services.AddHttpClient(options.ClientName);

        services.TryAddSingleton<ITelegramBotClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(options.ClientName);

            return new TelegramBotClient(options.BotToken, httpClient);
        });

        return services;
    }
}