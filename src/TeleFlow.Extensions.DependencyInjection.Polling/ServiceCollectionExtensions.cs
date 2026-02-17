using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Extensions.DependencyInjection.Configuration;
using TeleFlow.Extensions.DependencyInjection.Polling.Configuration;
using TeleFlow.Extensions.DependencyInjection.Polling.Services;
using Telegram.Bot;

namespace TeleFlow.Extensions.DependencyInjection.Polling;

public static class ServiceCollectionExtensions
{
    public static void AddTeleFlowPolling(this IServiceCollection services, TeleFlowPollingConfiguration pollingOptions, Action<TeleFlowConfiguration> options)
    {
        services.AddTeleFlow(options);
        services.TryAddTeleFlowTelegramClient(pollingOptions);

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