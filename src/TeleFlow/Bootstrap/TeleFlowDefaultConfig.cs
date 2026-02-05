using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Builders;
using TeleFlow.Commands;
using TeleFlow.Commands.Stateless;
using TeleFlow.Factories;
using TeleFlow.Middlewares;
using TeleFlow.Middlewares.CommandInterpreters;
using TeleFlow.Middlewares.CommandInterpreters.MultiStep;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;

namespace TeleFlow.Bootstrap;

internal static class TeleFlowDefaultConfig
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

    internal static void ConfigureCommandsDefault(CommandRegistryBuilder builder)
    {
        builder.AddOrReplace("/start", () => new SendTextCommand("Welcome to TeleFlow! Create your commands via options.ConfigureCommands(..)"));
    }
}