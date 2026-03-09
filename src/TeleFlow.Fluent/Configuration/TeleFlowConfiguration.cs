using TeleFlow.Fluent.Builders.Commands;
using TeleFlow.Fluent.Builders.Pipeline;

namespace TeleFlow.Fluent.Configuration;

public class TeleFlowConfiguration
{
    public static TeleFlowConfiguration Default
        => new(TeleFlowDefaultConfigInternal.ConfigurePipelineDefault, TeleFlowDefaultConfigInternal.ConfigureCommandsDefault);


    protected internal Action<MiddlewarePipelineBuilder> MiddlewareConfiguration { get; private set; }

    protected internal Action<CommandRouterBuilder> CommandRoutersConfiguration { get; private set; }

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

    public TeleFlowConfiguration ConfigureCommandRouters(Action<CommandRouterBuilder> options)
    {
        var current = CommandRoutersConfiguration;

        CommandRoutersConfiguration = (builder) =>
        {
            current.Invoke(builder);
            options.Invoke(builder);    
        };

        return this;
    }

    private TeleFlowConfiguration(Action<MiddlewarePipelineBuilder> middlewareConfiguration, Action<CommandRouterBuilder> commandRoutersConfiguration)
    {
        MiddlewareConfiguration = middlewareConfiguration;
        CommandRoutersConfiguration = commandRoutersConfiguration;
    }
}