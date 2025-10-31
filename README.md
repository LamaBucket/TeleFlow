# 🧠 **TeleFlow**

*A modern, extensible Telegram bot framework for .NET — built for structured conversations, FSM-driven flows, and developer joy.*
---

## ✨ Key Features

* **Multi-Step Commands (FSM)** – build rich conversational flows without state spaghetti.
* **Built-in DatePicker** – interactive inline calendar, no more “enter date in DD/MM/YYYY.”
* **Interceptor & Validator pipeline** – plug checks or transformations before handlers.
* **Navigation between commands** – move seamlessly through dialog stages.
* **Async first** – fully asynchronous execution pipeline.
* **Fluent configuration via lambdas** – ASP.NET-Core-style setup for familiarity.
* **Photo and Media support** – receive, process, and send images out of the box.

---

## 🚀 Quick Start

### 1️⃣ Install

```bash
dotnet add package TeleFlow
```

### 2️⃣ Create your bot entry point

```csharp

using demo.Services;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Services;
using Telegram.Bot;
using TeleFlow.Services.Markup;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

string BOT_TOKEN = System.IO.File.ReadAllText("bot-token.txt");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message>, UniversalMessageServiceFactory>();
builder.Services.AddSingleton<IMessageServiceFactory<string>, UniversalMessageServiceFactory>();
builder.Services.AddSingleton<IMessageServiceFactory<ImageMessageServiceMessage>, UniversalMessageServiceFactory>();



builder.Services.AddSingleton<IReplyMarkupManagerFactory, ReplyMarkupManagerFactory>();
builder.Services.AddSingleton<InlineMarkupManagerFactory, DemoInlineMarkupManagerFactory>();

builder.Services.AddSingleton<IMediaDownloaderServiceFactory, MediaDownloaderServiceFactory>();

builder.Services.AddSingleton<IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs>, UpdateDistributorArgsBuilder>();

builder.Services.AddSingleton<UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs>, DemoUpdateDistributorFactory>();

builder.Services.AddHttpClient("tgwebhook")
                .RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(BOT_TOKEN, httpClient));

builder.Services.ConfigureTelegramBotMvc();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


```

### 3️⃣ Define your first command

```csharp
using TeleFlow.Builders;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Services;

namespace demo;

public class DemoUpdateDistributorFactory : UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs>
{

    protected override void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> builder)
    {
        builder.WithLambda("/start", (args) => {
            return new SendTextCommand(args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "Welcome to the bot!");
        });
    }

    public DemoUpdateDistributorFactory(IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs> updateDistributorArgsBuilder) : base(updateDistributorArgsBuilder)
    {
    }
}
```

---

## 🧩 Building Multi-Step Commands (FSM)

TeleFlow lets you model interactive dialogs as **state machines**:

```csharp

public class DemoUpdateDistributorFactory : UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs>
{

    protected override void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> builder)
    {
        builder
        .WithMultiStep<DemoViewModel>("/multistep", options =>
        {
            options
            .SetDefaultStateValue(new())
            .WithValidation(options =>
            {
                options
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new ContactShareStepCommand(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.ReplyMarkupManager, (userInput) =>
                    {
                        args.State.CurrentValue.PhoneNumber = userInput.PhoneNumber;
                    }, "Please Share Your Phone. This will NOT go anywhere", "Share My Phone",
                    new PhoneNumberBelongsToUserValidator(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.ChatIdProvider, "The input was invalid", "The phone number does not belong to you."),
                    next);
                }, "Phone Number")
                .WithStepWithValidationLambdaFactoryGoBackButton((args, next, validator) =>
                {
                    return new TextInputStepCommand(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "Please enter your full name", async (message) =>
                    {
                        args.State.CurrentValue.UserFullName = message;
                    }, next, validator);
                }, "User Full Name")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new EnumValueSelectionStepCommand<DemoEnum>(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, "Please select one of the values",
                    async (userInput) =>
                    {
                        args.State.CurrentValue.LibraryRating = userInput;
                    },
                    (enumValue) =>
                    {
                        if (enumValue == DemoEnum.None)
                            return null;

                        return enumValue.ToString();
                    }, new(), next);
                }, "Library Rating")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new ListValueSelectionStepCommand<DemoListObject>(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, "Please select the value.",
                    async (userInput) =>
                    {
                        args.State.CurrentValue.ListObject = userInput;
                    },
                    new(3),
                    async () =>
                    {
                        return new List<DemoListObject>() { new() { DisplayName = "List Object 1", Value = "Value 1" }, new() { DisplayName = "List Object 2", Value = "Value 2" } };
                    },
                    (listObject) =>
                    {
                        return listObject.DisplayName;
                    }, next);
                }, "List Object Selection");
            })
            .WithLambdaResult(args =>
            {
                return new LambdaHandler<DemoViewModel>(async (vm) =>
                {
                    StringBuilder sb = new();

                    sb.AppendLine("You have successfully completed the multi step command. Here is the data you entered:");
                    sb.AppendLine($"Full Name: {vm.UserFullName}");
                    sb.AppendLine($"Library Rating: {vm.LibraryRating}");
                    sb.AppendLine($"List Object: {vm.ListObject.DisplayName} with value {vm.ListObject.Value}");
                    sb.AppendLine($"Phone: {vm.PhoneNumber}");

                    await args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString.SendMessage(sb.ToString());
                    await args.BuildTimeArgs.Navigator.Handle("/start");
                });
            });
        })
    }

    public DemoUpdateDistributorFactory(IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs> updateDistributorArgsBuilder) : base(updateDistributorArgsBuilder)
    {
    }
}

```
---

## 📅 Using the DatePicker

Inline calendar selection is as simple as:

```csharp

public class DemoUpdateDistributorFactory : UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs>
{

    protected override void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> builder)
    {
        builder
        .WithMultiStep<DemoViewModel>("/multistep", options =>
        {
            options
            .SetDefaultStateValue(new())
            .WithValidation(options =>
            {
                options
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new DateSelectionStepCommand(next, args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, (date) =>
                    {
                        args.State.CurrentValue.SelectedDate = date;
                    }, "Pick a date", new(DateOnly.FromDateTime(DateTime.Today).AddYears(-18), TeleFlow.Services.DateSelectionView.YearSelection), null, new(DateOnly.FromDateTime(DateTime.Today).AddYears(-14), "You are too young!"));
                }, "Date");
            })
            .WithLambdaResult(args =>
            {
                return new LambdaHandler<DemoViewModel>(async (vm) =>
                {
                    StringBuilder sb = new();

                    sb.AppendLine("You have successfully completed the multi step command. Here is the data you entered:");
                    sb.AppendLine($"Date: {vm.SelectedDate.ToShortDateString()}");

                    await args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString.SendMessage(sb.ToString());
                    await args.BuildTimeArgs.Navigator.Handle("/start");
                });
            });
        })
    }

    public DemoUpdateDistributorFactory(IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs> updateDistributorArgsBuilder) : base(updateDistributorArgsBuilder)
    {
    }
}

```

---

## 🧭 Navigation & Interceptors

Interceptors run **before** your handler executes — useful for auth, validation, or logging.

---


## 🧠 Concepts in TeleFlow

| Concept                       | Purpose                                                |
| ----------------------------- | ------------------------------------------------------ |
| **Command Handler**           | Encapsulates single user action.                       |
| **MultiStep Command**         | Defines sequential interaction with user state.        |
| **Navigator**                 | Moves user between commands/states.                    |
| **Interceptors / Validators** | Pre-processing logic.                                  |
| **DatePicker**                | Inline UI component for dates.                         |
| **Factory Builder**           | Internal system creating command handlers dynamically. |

<pre> ```mermaid flowchart TD A[Telegram Update] --> B[Interceptors<br>(logging, throttling...)] B --> C[Validators<br>(auth checks, input validation)] C --> D[CommandFactory Resolution<br>(via builder)] D --> E[ICommandHandler<br>(executes business logic)] E --> F[Navigator (optional)<br>moves between steps] F --> G[Bot sends response/update] ``` </pre>

---


## 🧰 Advanced Topics

* **Error Handling & Retry Policies**
* **Dependency Injection**
* **Custom Interceptors**
* **Testing Command Flows**

---

## 📦 Roadmap

* [ ] Inline Keyboards API V2
* [ ] JSON-based Dialog Serialization
* [ ] Redis state storage extension (`TeleFlow.Redis`)
* [ ] Admin Panel Toolkit

---

## 💬 Community

* Telegram: [@TeleFlowDev](https://t.me/TeleFlowDev)
* GitHub Discussions: [github.com/lamabucket/TeleFlow/discussions](https://github.com/yourusername/TeleFlow/discussions)

---

## 🧩 Contributing

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/foo`)
3. Commit changes (`git commit -m 'Add foo'`)
4. Push (`git push origin feature/foo`)
5. Open a PR

---

## 🧠 License

MIT © 2025 — Created by Gleb Bannyy

---

## 🪄 One-Sentence Pitch (for NuGet and socials)

> **TeleFlow** — Fluent and async Telegram bot framework for .NET with FSM, interceptors & built-in DatePicker.

---

### ✅ Summary of what you need to add

| Placeholder                 | What to include                                     |
| --------------------------- | --------------------------------------------------- |
| **Program.cs snapshot**     | Minimal bot initialization & configuration pipeline |
| **StartCommand**            | Simplest `ICommandHandler` example                  |
| **MultiStep example**       | Step-by-step FSM collecting data                    |
| **DatePicker example**      | Show inline calendar interaction                    |
| **Interceptor example**     | Auth or validation interceptor                      |
| **Config snapshot**         | Real lambda setup using your builder                |
| **Flow diagram (optional)** | How Update → Factory → Handler pipeline works       |
| **Custom Interceptor + DI** | Example of advanced extensibility                   |
| **CONTRIBUTING excerpt**    | How to run/test locally                             |
