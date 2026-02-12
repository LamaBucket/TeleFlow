using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Configuration;
using TeleFlow.Commands.Instant;
using TeleFlow.Pipeline.Configuration;
using TeleFlow.Pipeline.Middlewares;
using TeleFlow.Pipeline.Middlewares.CommandInterpreters;
using TeleFlow.Pipeline.Middlewares.CommandInterpreters.FlowStep;

namespace TeleFlow.DependencyInjection.Internal;

internal static class TeleFlowDefaultConfigInternal
{
    internal static void ConfigurePipelineDefault(MiddlewarePipelineBuilder options)
    {
        options
        .UseUpdateReceiver<UpdateReceiverMiddleware>()
        .UseCommandRouter<CommandRoutingMiddleware>()
        .UseCommandExecutor<CommandExecutionMiddleware>();
        
        
        options.Interpreters
        .WithInterpreterMiddleware<ExitCommandMiddleware>()
        .WithInterpreterMiddleware<GoToMultiStepMiddleware>()
        .WithInterpreterMiddleware<HoldOnMultiStepMiddleware>()
        .UseTerminalCommandInterpreter((sp) => {
            var store = sp.GetRequiredService<IChatSessionStore>();
            return new DefaultCommandInterpreter(store);
        });
        
        options.Interpreters.WithNavigateInterpreter(3);
    }

    internal static void ConfigureCommandsDefault(CommandResolversBuilder builder)
    {
        builder.AddOrReplace("/start", () => new SendMessageCommand("Welcome to TeleFlow! Create your commands via options.ConfigureCommands(..)"));
    }
}