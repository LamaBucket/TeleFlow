using TeleFlow.Builders;

namespace TeleFlow.Bootstrap;

public class TeleFlowConfiguration
{
    public static TeleFlowConfiguration Default
        => new(TeleFlowDefaultConfig.ConfigurePipelineDefault, TeleFlowDefaultConfig.ConfigureCommandsDefault);


    protected internal Action<MiddlewarePipelineBuilder> MiddlewareConfiguration { get; private set; }

    protected internal Action<CommandRegistryBuilder> CommandRegistryConfiguration { get; private set; }

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

    public TeleFlowConfiguration ConfigureCommandRegistry(Action<CommandRegistryBuilder> options)
    {
        var current = CommandRegistryConfiguration;

        CommandRegistryConfiguration = (builder) =>
        {
            current.Invoke(builder);
            options.Invoke(builder);    
        };

        return this;
    }

    private TeleFlowConfiguration(Action<MiddlewarePipelineBuilder> middlewareConfiguration, Action<CommandRegistryBuilder> commandRegistryConfiguration)
    {
        MiddlewareConfiguration = middlewareConfiguration;
        CommandRegistryConfiguration = commandRegistryConfiguration;
    }
}