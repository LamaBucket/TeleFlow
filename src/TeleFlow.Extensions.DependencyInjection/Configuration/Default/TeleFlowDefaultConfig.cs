using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Stateless;
using TeleFlow.Core.Pipeline;
using TeleFlow.Core.Pipeline.CommandInterpreters;
using TeleFlow.Core.Pipeline.CommandInterpreters.Stateful;
using TeleFlow.Extensions.DependencyInjection.Builders.Commands;
using TeleFlow.Extensions.DependencyInjection.Builders.Pipeline;

namespace TeleFlow.Extensions.DependencyInjection.Configuration.Default;

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
        .WithInterpreterMiddleware<GoToStatefulMiddleware>()
        .WithInterpreterMiddleware<HoldOnStatefulMiddleware>()
        .UseTerminalCommandInterpreter((sp) => {
            var store = sp.GetRequiredService<IChatSessionStateStore>();
            return new DefaultCommandInterpreter(store);
        });
        
        options.Interpreters.WithNavigateInterpreter(3);
    }

    internal static void ConfigureCommandsDefault(CommandRouterBuilder builder)
    {
        builder.AddOrReplace("/start", () => new SendMessageCommand("Welcome to TeleFlow! Create your commands via options.ConfigureCommands(..)"));
    }
}