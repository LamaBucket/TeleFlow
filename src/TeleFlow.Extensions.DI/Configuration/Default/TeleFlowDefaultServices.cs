using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Factories;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Extensions.DI.Builders.Commands;
using TeleFlow.Extensions.DI.Builders.Pipeline;
using TeleFlow.Extensions.DI.Implementations.Engine;
using TeleFlow.Extensions.DI.Implementations.State.Chat;
using TeleFlow.Extensions.DI.Implementations.State.Step;
using TeleFlow.Extensions.DI.Implementations.Transport.Callbacks;
using TeleFlow.Extensions.DI.Implementations.Transport.Files;
using TeleFlow.Extensions.DI.Implementations.Transport.Messaging;
using Telegram.Bot;

namespace TeleFlow.Extensions.DI.Configuration.Default;

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

    internal static void ConfigureCommandRouters(this IServiceCollection services, Action<CommandRouterBuilder> options)
    {
        var builder = new CommandRouterBuilder();
        options.Invoke(builder);

        var (sessionRouter, navigatedRouter) = builder.Build();

        services.TryAddSingleton<ICommandFactory<ICommandHandler, ChatSession>>(sessionRouter);
        services.TryAddSingleton<ICommandFactory<ICommandHandler, NavigateCommandResult>>(navigatedRouter);
    }
}