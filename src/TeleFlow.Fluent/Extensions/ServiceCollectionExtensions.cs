using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Factories;
using TeleFlow.Fluent.Builders.Commands;
using TeleFlow.Fluent.Builders.Pipeline;
using TeleFlow.Fluent.Configuration;

namespace TeleFlow.Fluent.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTeleFlowPipeline(this IServiceCollection services, Action<TeleFlowConfiguration> configure)
    {
        var configuration = TeleFlowConfiguration.Default;
        
        configure(configuration);

        services.ConfigureMiddlewarePipeline(configuration.MiddlewareConfiguration);
        services.ConfigureCommandRouters(configuration.CommandRoutersConfiguration);
    }

    private static void ConfigureMiddlewarePipeline(this IServiceCollection services, Action<MiddlewarePipelineBuilder> options)
    {
        services.TryAddSingleton(sp =>
        {
            MiddlewarePipelineBuilder builder = new();
            options.Invoke(builder);

            return builder.Build(sp);
        });
    }

    private static void ConfigureCommandRouters(this IServiceCollection services, Action<CommandRouterBuilder> options)
    {
        var builder = new CommandRouterBuilder();
        options.Invoke(builder);

        var (sessionRouter, navigatedRouter) = builder.Build();

        services.TryAddSingleton<ICommandFactory<ICommandHandler, ChatSession>>(sessionRouter);
        services.TryAddSingleton<ICommandFactory<ICommandHandler, NavigateCommandResult>>(navigatedRouter);
    }
}