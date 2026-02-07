using TeleFlow.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Bootstrap;
using TeleFlow.Builders;
using TeleFlow.Middlewares.CommandInterpreters;
using TeleFlow.Factories;
using TeleFlow.Commands;
using TeleFlow.Models.CommandResults;
using TeleFlow.Services;
using TeleFlow.Services.Messaging;
using TeleFlow.Models.Messaging;

namespace TeleFlow;

public static class LibraryExtensions
{
    public static void AddTeleFlow(this IServiceCollection services, Action<TeleFlowConfiguration> options)
    {
        TeleFlowDefaultServices.ConfigureServices(services);

        var configuration = TeleFlowConfiguration.Default;

        options.Invoke(configuration);

        TeleFlowDefaultServices.ConfigureMiddlewarePipeline(services, configuration.MiddlewareConfiguration);

        TeleFlowDefaultServices.ConfigureCommandRegistries(services, configuration.CommandRegistryConfiguration);
    }


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

    public static long GetChatId(this Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id ?? throw new NullReferenceException("Unable To Retrieve Chat Id From Message"),
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id ?? throw new NullReferenceException("Unable To Retrieve Chat Id From CallbackQuery"),
            _ => throw new InvalidOperationException("This MessageType is not supported"),
        };
    }

    public static string? GetCommandName(this Update args)
    {
        switch(args.Type)
        {
            case UpdateType.Message:
                return args.Message?.Text;
            case UpdateType.CallbackQuery:
                return GetCommandNameFromCallbackQuery(args.CallbackQuery ?? throw new ArgumentNullException($"The callback query in {nameof(args)} was not provided"));
        }
        throw new InvalidOperationException("Unknown update type");
    }

    private static string? GetCommandNameFromCallbackQuery(CallbackQuery query)
    {
        string queryData = query.Data ?? throw new ArgumentNullException($"The data in {nameof(query)} was not provided");

        var args = JsonConvert.DeserializeObject<UniversalCommandFactoryViewModel>(queryData);

        return args?.CommandToExecute;
    }

    public static Task<Message> SendMessage(this IMessageSender messageService, string message)
    {
        OutgoingMessage msg = new(){ Text = message };
        
        return messageService.SendMessage(msg);
    } 
}