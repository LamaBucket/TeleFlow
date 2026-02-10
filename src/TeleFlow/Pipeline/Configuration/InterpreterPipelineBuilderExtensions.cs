using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Builders;
using TeleFlow.Commands;
using TeleFlow.Factories;
using TeleFlow.Middlewares.CommandInterpreters;
using TeleFlow.Models.CommandResults;
using TeleFlow.Services;

namespace TeleFlow.Pipeline.Configuration;

public static class InterpreterPipelineBuilderExtensions
{
    public static InterpreterPipelineBuilder WithNavigateInterpreter(this InterpreterPipelineBuilder builder, int navigateDepth)
    {
        if(navigateDepth < 0)
            throw new Exception();
        
        if(navigateDepth > 0)
        {
            var navigatedCommandInterpreterBuilder = builder.Clone();
            navigatedCommandInterpreterBuilder.WithNavigateInterpreter(navigateDepth - 1);

            builder.WithInterpreterMiddleware((sp, next) =>
            {
                var cmdFactory                  = sp.GetRequiredService<ICommandFactory<ICommandHandler, NavigateCommandResult>>(); 
                var store                       = sp.GetRequiredService<IChatSessionStore>();
                var navigatedCommandInterpreter = navigatedCommandInterpreterBuilder.Build(sp);

                return new NavigateCommandMiddleware(next, navigatedCommandInterpreter, cmdFactory, store);
            });
        }

        return builder;
    }
}