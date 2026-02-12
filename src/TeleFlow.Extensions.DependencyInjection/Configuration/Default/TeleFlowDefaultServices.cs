using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Abstractions.Callbacks;
using TeleFlow.Abstractions.Interactivity;
using TeleFlow.Abstractions.Messaging;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands;
using TeleFlow.Commands.Configuration;
using TeleFlow.Commands.Factories;
using TeleFlow.Commands.Results;
using TeleFlow.Implementations.Callbacks;
using TeleFlow.Implementations.Interactivity;
using TeleFlow.Implementations.Messaging;
using TeleFlow.Implementations.Sessions;
using TeleFlow.Pipeline.Configuration;
using Telegram.Bot;

namespace TeleFlow.DependencyInjection.Internal;

internal static class TeleFlowDefaultServicesInternal
{
    internal static void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton<IChatSessionStore, InMemoryChatSessionStore>();

        services.TryAddDefaultChatIdManager();

        services.TryAddDefaultCallbackEncoder();
        services.TryAddInMemoryInteractiveStateStore();

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

    internal static IServiceCollection TryAddDefaultCallbackEncoder(this IServiceCollection services)
    {
        services.TryAddSingleton<ICallbackCodec, DefaultCallbackEncoder>();
        return services;
    }

    internal static IServiceCollection TryAddInMemoryInteractiveStateStore(this IServiceCollection services)
    {
        services.TryAddSingleton<IInteractiveStateStore, InMemoryInteractiveStateStore>();
        return services;
    }

    internal static void ConfigureMiddlewarePipeline(this IServiceCollection services, Action<MiddlewarePipelineBuilder> options)
    {
        services.TryAddSingleton(sp =>
        {
            MiddlewarePipelineBuilder builder = new();
            options.Invoke(builder);

            return builder.Build(sp);
        });
    }

    internal static void ConfigureCommandRegistries(this IServiceCollection services, Action<CommandResolversBuilder> options)
    {
        var builder = new CommandResolversBuilder();
        options.Invoke(builder);

        var (sessionRegistry, navigatedRegistry) = builder.Build();

        services.TryAddSingleton<ICommandFactory<ICommandHandler, ChatSession>>(sessionRegistry);
        services.TryAddSingleton<ICommandFactory<ICommandHandler, NavigateCommandResult>>(navigatedRegistry);
    }
}