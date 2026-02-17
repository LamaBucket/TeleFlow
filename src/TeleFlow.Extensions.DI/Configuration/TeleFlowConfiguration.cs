using TeleFlow.Extensions.DI.Builders.Commands;
using TeleFlow.Extensions.DI.Builders.Pipeline;
using TeleFlow.Extensions.DI.Configuration.Default;

namespace TeleFlow.Extensions.DI.Configuration;

public class TeleFlowConfiguration
{
    public static TeleFlowConfiguration Default
        => new(TeleFlowDefaultConfigInternal.ConfigurePipelineDefault, TeleFlowDefaultConfigInternal.ConfigureCommandsDefault);


    protected internal Action<MiddlewarePipelineBuilder> MiddlewareConfiguration { get; private set; }

    protected internal Action<CommandRouterBuilder> CommandRegistryConfiguration { get; private set; }

    public TeleFlowConfiguration ConfigureMiddlewarePipeline(Action<MiddlewarePipelineBuilder> options)
    {
        var current = MiddlewareConfiguration;

        MiddlewareConfiguration = (builder) =>
        {
            current.Invoke(builder);
            options.Invoke(builder);
        };

        return this;
    }

    public TeleFlowConfiguration ConfigureCommandRegistry(Action<CommandRouterBuilder> options)
    {
        var current = CommandRegistryConfiguration;

        CommandRegistryConfiguration = (builder) =>
        {
            current.Invoke(builder);
            options.Invoke(builder);    
        };

        return this;
    }

    private TeleFlowConfiguration(Action<MiddlewarePipelineBuilder> middlewareConfiguration, Action<CommandRouterBuilder> commandRegistryConfiguration)
    {
        MiddlewareConfiguration = middlewareConfiguration;
        CommandRegistryConfiguration = commandRegistryConfiguration;
    }
}