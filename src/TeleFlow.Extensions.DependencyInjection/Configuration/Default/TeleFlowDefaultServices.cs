using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Factories;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Extensions.DependencyInjection.Builders.Commands;
using TeleFlow.Extensions.DependencyInjection.Builders.Pipeline;
using TeleFlow.Extensions.DependencyInjection.Implementations.Engine;
using TeleFlow.Extensions.DependencyInjection.Implementations.State.Chat;
using TeleFlow.Extensions.DependencyInjection.Implementations.State.Step;
using TeleFlow.Extensions.DependencyInjection.Implementations.Transport.Callbacks;
using TeleFlow.Extensions.DependencyInjection.Implementations.Transport.Messaging;
using Telegram.Bot;

namespace TeleFlow.Extensions.DependencyInjection.Configuration.Default;

internal static class TeleFlowDefaultServicesInternal
{
    internal static void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton<IChatSessionStateStore, InMemoryChatSessionStore>();

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
        services.TryAddSingleton<ICallbackCodec, DefaultCallbackCodec>();
        services.TryAddSingleton<ICallbackActionParser, DefaultCallbackActionParser>();
        return services;
    }

    internal static IServiceCollection TryAddInMemoryInteractiveStateStore(this IServiceCollection services)
    {
        services.TryAddSingleton<IStepStateStore, InMemoryStepStateStore>();
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

    internal static void ConfigureCommandRegistries(this IServiceCollection services, Action<CommandRouterBuilder> options)
    {
        var builder = new CommandRouterBuilder();
        options.Invoke(builder);

        var (sessionRegistry, navigatedRegistry) = builder.Build();

        services.TryAddSingleton<ICommandFactory<ICommandHandler, ChatSession>>(sessionRegistry);
        services.TryAddSingleton<ICommandFactory<ICommandHandler, NavigateCommandResult>>(navigatedRegistry);
    }
}