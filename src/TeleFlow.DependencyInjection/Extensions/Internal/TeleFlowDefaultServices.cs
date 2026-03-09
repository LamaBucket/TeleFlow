using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.DependencyInjection.Implementations.Engine;
using TeleFlow.DependencyInjection.Implementations.State.Chat;
using TeleFlow.DependencyInjection.Implementations.State.Step;
using TeleFlow.DependencyInjection.Implementations.Transport.Callbacks;
using TeleFlow.DependencyInjection.Implementations.Transport.Files;
using TeleFlow.DependencyInjection.Implementations.Transport.Messaging;
using Telegram.Bot;

namespace TeleFlow.DependencyInjection.Extensions.Internal;

internal static class TeleFlowDefaultServicesInternal
{
    internal static void ConfigureServices(IServiceCollection services)
    {
        services.TryAddSingleton<IChatSessionStore, InMemoryChatSessionStore>();

        services
        .TryAddDefaultChatIdManager()
        
        .TryAddDefaultMessageServices()

        .TryAddDefaultCallbackEncoder()
        .TryAddDefaultFileReferenceExtractors()

        .TryAddInMemoryInteractiveStateStore();
    }

    private static IServiceCollection TryAddDefaultChatIdManager(this IServiceCollection services)
    {
        services.TryAddScoped<InMemoryChatIdManager>();
        services.TryAddScoped<IChatIdSetter>(sp => sp.GetRequiredService<InMemoryChatIdManager>());
        services.TryAddScoped<IChatIdProvider>(sp => sp.GetRequiredService<InMemoryChatIdManager>());

        return services;
    }

    private static IServiceCollection TryAddDefaultMessageServices(this IServiceCollection services)
    {
        services.TryAddScoped<IMessageSendService>(sp =>
        {
            var chatIdProvider = sp.GetRequiredService<IChatIdProvider>();
            var botClient = sp.GetRequiredService<ITelegramBotClient>(); 

            var chatId = chatIdProvider.GetChatId();

            return new DefaultMessageSender(botClient, chatId);
        });

        services.TryAddScoped<IMessageEditService>(sp =>
        {
            var chatIdProvider = sp.GetRequiredService<IChatIdProvider>();
            var botClient = sp.GetRequiredService<ITelegramBotClient>(); 

            var chatId = chatIdProvider.GetChatId();

            return new DefaultMessageEditor(botClient, chatId);
        });

        services.TryAddScoped<IMessageDeleteService>(sp =>
        {
            var chatIdProvider = sp.GetRequiredService<IChatIdProvider>();
            var botClient = sp.GetRequiredService<ITelegramBotClient>(); 

            var chatId = chatIdProvider.GetChatId();

            return new DefaultMessageDeleteService(chatId, botClient);
        });

        return services;
    }

    private static IServiceCollection TryAddDefaultCallbackEncoder(this IServiceCollection services)
    {
        services.TryAddSingleton<ICallbackCodec, DefaultCallbackCodec>();
        services.TryAddSingleton<ICallbackActionParser, DefaultCallbackActionParser>();
        return services;
    }

    private static IServiceCollection TryAddDefaultFileReferenceExtractors(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IFileReferenceExtractor, ImageFileReferenceExtractor>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IFileReferenceExtractor, VideoFileReferenceExtractor>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IFileReferenceExtractor, AudioFileReferenceExtractor>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IFileReferenceExtractor, DocumentFileReferenceExtractor>());
        
        return services;
    }

    private static IServiceCollection TryAddInMemoryInteractiveStateStore(this IServiceCollection services)
    {
        services.TryAddSingleton<IStepStateStore, InMemoryStepStateStore>();
        return services;
    }
}