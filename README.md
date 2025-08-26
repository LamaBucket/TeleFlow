# Telegram.Bot.Extensions.Handlers

A modular, extensible, and easy-to-use framework for handling Telegram bot updates in C#.  
This library helps you organize your bot's command and update handling logic using a clean, composable architecture.

---

## Features

- **Command Pattern** for update handling
- **Multi-step commands** and state management
- **Builder-based configuration**
- **Per-chat update routing**
- **Easy integration with Telegram.Bot**

---

## Quick Start

### 1. Install Dependencies

Add the following NuGet packages to your project:

- [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

### 2. Setup Update Handling

You can set up the entire update handling process in just a few lines:

```csharp
using LisBot.Common.Telegram;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Services;

// 1. Configure your message services, authentication, etc.
IMessageService<string> messageServiceString = ...;
IMessageService<Message> messageService = ...;
IMessageService<Tuple<string, KeyboardButton>> messageServiceWithReplyButton = ...;
IReplyMarkupManager replyMarkupManager = ...;
IAuthenticationService authenticationService = ...;

// 2. Build your command factory
var commandFactoryBuilder = new UpdateListenerCommandFactoryBuilder()
    .WithSendText("/start", "Welcome to the bot!")
    .WithSendText("/help", "Here is how to use the bot...");

// 3. Create the update distributor factory
var updateDistributorFactory = new UpdateDistributorFactory(
    chatIdProvider => messageService,
    chatIdProvider => messageServiceString,
    chatIdProvider => messageServiceWithReplyButton,
    chatIdProvider => replyMarkupManager,
    chatIdProvider => authenticationService,
    builder => builder // configure your commands here
);

// 4. Create the update distributor (singleton)
var updateDistributor = updateDistributorFactory.Create();

// 5. Handle incoming updates
await updateDistributor.Handle(update);
```

### 3. Add Your Own Commands

You can easily add multi-step commands, authentication, and conditional logic using the builder pattern:

```csharp
commandFactoryBuilder
    .WithMultiStep<MyStateType>("/wizard", builder => {
        builder
            .SetDefaultStateValue(new MyStateType())
            .WithoutValidation(stepBuilder => {
                // Add steps here
            });
    });
```

---

## Why Use This Library?

- **Minimal boilerplate**: Focus on your bot logic, not on plumbing.
- **Extensible**: Add new command types, steps, and handlers easily.
- **Per-chat isolation**: Each chat gets its own handler chain.
- **Testable**: Decouple your logic for easier testing.

---

## Project Structure

- [`src/Telegram.Bot.Extensions.Handlers/`](src/Telegram.Bot.Extensions.Handlers/)
    - Core interfaces and classes for update handling
    - Builders, factories, and services for composing your bot logic

---

## License

MIT

---

## Contributing

Pull requests and issues are welcome!