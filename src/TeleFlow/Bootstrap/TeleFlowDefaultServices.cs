using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Builders;
using TeleFlow.Commands;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Services;
using TeleFlow.Services.Defaults;
using TeleFlow.Services.Defaults.Messaging;
using TeleFlow.Services.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleFlow.Bootstrap;

public static class TeleFlowDefaultServices
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton<IChatSessionStore, InMemoryChatSessionStore>();

        services.TryAddDefaultChatIdManager();

        services.TryAddDefaultMessageServices();
    }

    private static void TryAddDefaultChatIdManager(this IServiceCollection services)
    {
        services.TryAddScoped<InMemoryChatIdManager>();
        services.TryAddScoped<IChatIdSetter>(sp => sp.GetRequiredService<InMemoryChatIdManager>());
        services.TryAddScoped<IChatIdProvider>(sp => sp.GetRequiredService<InMemoryChatIdManager>());
    }

    private static void TryAddDefaultMessageServices(this IServiceCollection services)
    {
        services.TryAddScoped<IMessageSender>(sp =>
        {
            var chatIdProvider = sp.GetRequiredService<IChatIdProvider>();
            var botClient = sp.GetRequiredService<ITelegramBotClient>(); 

            var chatId = chatIdProvider.GetChatId();

            return new DefaultMessageSender(botClient, chatId);
        });

        services.TryAddScoped<IMessageEditor>(sp =>
        {
            var chatIdProvider = sp.GetRequiredService<IChatIdProvider>();
            var botClient = sp.GetRequiredService<ITelegramBotClient>(); 

            var chatId = chatIdProvider.GetChatId();

            return new DefaultMessageEditor(botClient, chatId);
        });
    }

    public static void ConfigureMiddlewarePipeline(this IServiceCollection services, Action<MiddlewarePipelineBuilder> options)
    {
        services.TryAddSingleton(sp =>
        {
            MiddlewarePipelineBuilder builder = new();
            options.Invoke(builder);

            return builder.Build(sp);
        });
    }

    public static void ConfigureCommandRegistries(this IServiceCollection services, Action<CommandRegistryBuilder> options)
    {
        var builder = new CommandRegistryBuilder();
        options.Invoke(builder);

        var (sessionRegistry, navigatedRegistry) = builder.Build();

        services.TryAddSingleton<ICommandFactory<ICommandHandler, ChatSession>>(sessionRegistry);
        services.TryAddSingleton<ICommandFactory<ICommandHandler, NavigateCommandResult>>(navigatedRegistry);
    }
}