using TeleFlow.Commands.Configuration;
using TeleFlow.DependencyInjection.Internal;
using TeleFlow.Pipeline.Configuration;

namespace TeleFlow.DependencyInjection;

public class TeleFlowConfiguration
{
    public static TeleFlowConfiguration Default
        => new(TeleFlowDefaultConfigInternal.ConfigurePipelineDefault, TeleFlowDefaultConfigInternal.ConfigureCommandsDefault);


    protected internal Action<MiddlewarePipelineBuilder> MiddlewareConfiguration { get; private set; }

    protected internal Action<CommandResolversBuilder> CommandRegistryConfiguration { get; private set; }

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

    public TeleFlowConfiguration ConfigureCommandRegistry(Action<CommandResolversBuilder> options)
    {
        var current = CommandRegistryConfiguration;

        CommandRegistryConfiguration = (builder) =>
        {
            current.Invoke(builder);
            options.Invoke(builder);    
        };

        return this;
    }

    private TeleFlowConfiguration(Action<MiddlewarePipelineBuilder> middlewareConfiguration, Action<CommandResolversBuilder> commandRegistryConfiguration)
    {
        MiddlewareConfiguration = middlewareConfiguration;
        CommandRegistryConfiguration = commandRegistryConfiguration;
    }
}