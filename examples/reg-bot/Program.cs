using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using reg_bot;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Fluent.Extensions.Commands.Stateful.Steps;
using TeleFlow.Fluent.Extensions.Commands.Stateful.Steps.Filters;
using TeleFlow.Hosting.Polling;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? "";

var builder = Host.CreateDefaultBuilder();

UserStore store = new();

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
                        configure.AddTextInput(
                        "Hi! Please enter your username!", 
                        async (sp, input) => {
                            User u = new() { Username =  input };
                            store.SetUser(sp.ChatId, u);
                        }).RequireNoWhitespace();

                        configure.AddContactInput(
                        async (sp, contact) =>
                        {
                            User u = store.GetUser(sp.ChatId) ?? throw new Exception("User not found");
                            u.PhoneNumber = contact.PhoneNumber;
                            store.SetUser(sp.ChatId, u);
                        }, 
                        configure =>
                        {
                            configure.ShareContactButtonText = "Share!";
                        }).RequireSelfContact();
                        
                        configure.AddDateSelect(
                        async (sp, date) =>
                        {
                            User u = store.GetUser(sp.ChatId) ?? throw new Exception("User not found");
                            u.DateOfBirth = date;
                            store.SetUser(sp.ChatId, u);
                        }, 
                        configure =>
                        {
                            var today = DateTime.Today;
                            configure.Constraints.MaxDate = new DateOnly(today.Year + 10, 12, 1);
                            configure.Constraints.MinDate = new DateOnly(today.Year - 10, 1, 1); 
                        });

                        configure.AddSingleSelectEnum<Mood>(
                        (mood) => mood == Mood.Bad ? null : mood.ToString(), // This will remove bad mood from selection 
                        async (sp, mood) =>
                        {
                            User u = store.GetUser(sp.ChatId) ?? throw new Exception("User not found");
                            u.MoodToday = mood;
                            store.SetUser(sp.ChatId, u);
                        });

                    }, 
                    onCompleted: 
                    async (sp) =>
                    {
                        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
                        User u = store.GetUser(chatId) ?? throw new Exception("User not found");

                        var sb = new StringBuilder();
                        sb.AppendLine("Username: " + u.Username);
                        sb.AppendLine("Phone: " + u.PhoneNumber);
                        sb.AppendLine("DateOfBirth: " + u.DateOfBirth);
                        sb.AppendLine("MoodToday: " + u.MoodToday);
                        
                        var msgService = sp.GetRequiredService<IMessageSendService>();
                        await msgService.SendMessage(sb.ToString());

                        return CommandResult.Exit;
                    })
                    .EnableNavigation();
                })
    ));

await builder.Build().RunAsync();