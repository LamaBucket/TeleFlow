using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Fluent.Extensions.Commands.Stateful.Steps;
using TeleFlow.Fluent.Extensions.Commands.Stateful.Steps.Filters;
using TeleFlow.Hosting.Polling;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? "";

var builder = Host.CreateDefaultBuilder();

builder
.ConfigureLogging(logging =>
{
    logging.AddFilter((category, level) => level >= LogLevel.Error);
})
.ConfigureServices(services => services.AddTeleFlowPolling(botToken: token, configure: 
    options => 
    options
    .ConfigureCommandRouters(commands =>
    {
        commands.AddStateful("/start", configure =>
        {
            configure
                .AddTextInput(async (sp, input) =>
                {
                    var msgService = sp.GetRequired<IMessageSendService>();
                    await msgService.SendMessage(input);
                });
        }, onCompleted: async _ => new NavigateCommandResult("/start")).EnableNavigation();
    })
));

await builder.Build().RunAsync();