# TeleFlow

A modular, extensible, and easy-to-use framework for handling Telegram bot updates in C#.  
This library helps you organize your bot's command and update handling logic using a clean, composable architecture.

---

## Features

- **Command Pattern** for update handling
- **Multi-step commands** and state management
- **Builder-based configuration**
- **Per-chat update routing**
- **Authentication and conditional logic**
- **Easy integration with [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot)**
- **Extensible and testable**

---

## Quick Start

### 1. Install Dependencies

Add the following NuGet packages to your project:

- [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

---

### 2. Register Services and Configure Dependency Injection

Register the required services in your DI container (example for ASP.NET Core):

```csharp
// Program.cs
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register message services
builder.Services.AddSingleton<IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message>, UniversalMessageServiceFactory>();
builder.Services.AddSingleton<IMessageServiceFactory<string>, UniversalMessageServiceFactory>();

// Register reply markup manager and authentication
builder.Services.AddSingleton<IReplyMarkupManagerFactory, ReplyMarkupManagerFactory>();
builder.Services.AddSingleton<IAuthenticationServiceFactory, DemoAuthenticationServiceFactory>();

// Register the update distributor factory
builder.Services.AddSingleton<UpdateDistributorFactory, DemoUpdateDistributorFactory>();

// Register Telegram.Bot client
builder.Services.AddHttpClient("tgwebhook")
    .RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(BOT_TOKEN, httpClient));

// Register MVC integration
builder.Services.ConfigureTelegramBotMvc();
```

---

### 3. Configure Update Handling and Command Routing

Define your command routing and multi-step flows using the builder pattern.  
You can add multi-step commands, authentication, and conditional logic:

```csharp
// AppExtensions.cs
public static void ConfigureUpdateListenerForDemo(this UpdateListenerCommandFactoryBuilder builder)
{
    builder
        .WithSendText("/start", "Welcome to the bot!")
        .WithMultiStep<DemoViewModel>("/multistep", options =>
        {
            options
                .SetDefaultStateValue(new DemoViewModel())
                .WithValidation(stepBuilder =>
                {
                    stepBuilder
                        .WithStepWithValidationLambdaFactory((args, next) =>
                        {
                            return new ContactShareStepCommand(
                                args.UpdateListenerBuilderArgs.ReplyMarkupManager,
                                userInput => args.State.CurrentValue.PhoneNumber = userInput.PhoneNumber,
                                "Please Share Your Phone.",
                                "Share My Phone",
                                new PhoneNumberBelongsToUserValidator(
                                    args.UpdateListenerBuilderArgs.MessageServiceString,
                                    args.UpdateListenerBuilderArgs.ChatIdProvider,
                                    "The input was invalid",
                                    "The phone number does not belong to you."
                                ),
                                next
                            );
                        }, "Phone Number")
                        // Add more steps as needed...
                        .WithStepWithValidationLambdaFactoryGoBackButton((args, next, validator) =>
                        {
                            return new DateSelectionStepCommand(
                                next,
                                validator,
                                args.UpdateListenerBuilderArgs.MessageService,
                                date => args.State.CurrentValue.SelectedDate = date,
                                "Pick a date"
                            );
                        }, "Date");
                })
                .WithLambdaResult(args =>
                {
                    return new LambdaHandler<DemoViewModel>(async vm =>
                    {
                        await args.MessageServiceString.SendMessage("You have successfully completed the multi-step command. Here is the data you entered:");
                        await args.MessageServiceString.SendMessage($"Full Name: {vm.UserFullName}");
                        await args.MessageServiceString.SendMessage($"Library Rating: {vm.LibraryRating}");
                        await args.MessageServiceString.SendMessage($"List Object: {vm.ListObject.DisplayName} with value {vm.ListObject.Value}");
                        await args.MessageServiceString.SendMessage($"Date: {vm.SelectedDate.ToShortDateString()}");
                    });
                });
        });
}
```

---

### 4. Create the Update Distributor and Handle Updates

Instantiate the update distributor and handle incoming updates in your controller:

```csharp
// BotController.cs
[ApiController]
public class BotController : Controller
{
    private readonly UpdateDistributorFactory _updateDistributorFactory;
    private IHandler<Update> UpdateDistributor => _updateDistributorFactory.Create();

    [HttpPost("/botUpdate")]
    public async Task<ActionResult> HandleBotUpdate([FromBody] Update update)
    {
        await UpdateDistributor.Handle(update);
        return Ok();
    }

    public BotController(UpdateDistributorFactory updateDistributorFactory)
    {
        _updateDistributorFactory = updateDistributorFactory;
    }
}
```

---

## Why Use This Library?

- **Minimal boilerplate**: Focus on your bot logic, not on plumbing.
- **Extensible**: Add new command types, steps, and handlers easily.
- **Per-chat isolation**: Each chat gets its own handler chain.
- **Authentication and conditional flows**: Easily add authentication and conditional logic to your commands.
- **Testable**: Decouple your logic for easier testing.

---

## Project Structure

- [`src/TeleFlow/`](src/TeleFlow/)
    - Core interfaces and classes for update handling
    - Builders, factories, and services for composing your bot logic
- [`demo/`](demo/)
    - Example usage and demo implementation

---

## License

MIT

---

## Contributing

Pull requests and issues are welcome!