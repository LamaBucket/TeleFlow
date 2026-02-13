using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Factories;
using TeleFlow.Core.Pipeline.CommandInterpreters;

namespace TeleFlow.Extensions.DependencyInjection.Builders.Pipeline;

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
                var store                       = sp.GetRequiredService<IChatSessionStateStore>();
                var navigatedCommandInterpreter = navigatedCommandInterpreterBuilder.Build(sp);

                return new NavigateCommandMiddleware(next, navigatedCommandInterpreter, cmdFactory, store);
            });
        }

        return builder;
    }
}