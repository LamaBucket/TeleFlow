# TeleFlow

**TeleFlow** is a Telegram bot framework for .NET focused on **structured conversations**:  
an update pipeline + commands + chat sessions + multi-step flows (FSM-like) — without “state spaghetti”.

[![Docs](https://img.shields.io/badge/docs-online-blue)](https://lamabucket.github.io/TeleFlow/)
[![Release](https://img.shields.io/github/v/release/LamaBucket/TeleFlow)](https://github.com/LamaBucket/TeleFlow/releases)
[![Stars](https://img.shields.io/github/stars/LamaBucket/TeleFlow?style=social)](https://github.com/LamaBucket/TeleFlow)

---

## Why TeleFlow?

When bots grow, you usually end up with:
- scattered handlers,
- implicit state in random places,
- hard-to-debug flows.

TeleFlow makes the conversation model explicit:

- **Update pipeline** (middleware-style processing)
- **Commands** for application logic
- **ChatSession** as dialog state between updates
- **Stateful multi-step flows** built from reusable steps
- **CommandResult interpreters** controlling navigation, step transitions, and “hold on input”
- **Interceptors** for command-scoped cross-cutting concerns (auth, validation, logging)

---

## Quick Start (Polling)

### Prerequisites
- .NET 8 SDK
- Telegram bot token (via `@BotFather`)

### Install

```bash
dotnet new console -n MyTeleFlowBot
cd MyTeleFlowBot
dotnet add package TeleFlow
```

### Configure your token

Set environment variable `TELEGRAM_BOT_TOKEN`.

**Linux/macOS**
```bash
export TELEGRAM_BOT_TOKEN="123456:ABCDEF..."
```

**Windows (PowerShell)**
```powershell
setx TELEGRAM_BOT_TOKEN "123456:ABCDEF..."
```

### Program.cs (copy/paste)

This demo adds:
- `/start` → “Hello, World!”
- `/test`  → asks your name → asks rating (1..5) → prints the result

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TeleFlow.Extensions.DI.Polling;

using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Transport.Messaging;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
if (string.IsNullOrWhiteSpace(token))
    throw new InvalidOperationException("Environment variable TELEGRAM_BOT_TOKEN is not set.");

// WARNING: demo in-memory storage (for the first example).
// In production, use a proper ChatSession store / your DB / distributed cache.
Dictionary<long, string> names = new();
Dictionary<long, int> rates = new();

var host = Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        logging.AddFilter((category, level) => level >= LogLevel.Error);
    })
    .ConfigureServices(services =>
        services.AddTeleFlowPolling(
            botToken: token,
            teleFlow: options => options.ConfigureCommandRouters(commands =>
            {
                commands.AddSendText("/start", "Hello, World!");

                commands.AddMultiStep("/test", flow =>
                {
                    flow
                        .AddTextInput(
                            "Hey there! Please tell us your name.",
                            async (context, name) =>
                            {
                                var chatId = context.ChatId;
                                names[chatId] = name;
                            })
                        .AddSingleSelect(
                            "Please rate our library!",
                            new[] { 1, 2, 3, 4, 5 },
                            star => star.ToString(),
                            async (context, star) =>
                            {
                                var chatId = context.ChatId;
                                rates[chatId] = star;
                            });

                }, onCompleted: async sp =>
                {
                    // Finalization runs inside the current update scope.
                    var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();

                    var name = names[chatId];
                    var star = rates[chatId];

                    var msg = $"Your name is {name} and you rated us {star} star(s)!";

                    var sender = sp.GetRequiredService<IMessageSender>();
                    await sender.SendMessage(msg);

                    return CommandResult.Exit;
                });
            })
        )
    )
    .Build();

await host.RunAsync();
```

Run it:
```bash
dotnet run
```

---

## Built-in steps (Stateful flows)

TeleFlow ships with reusable steps you can compose in any order:
- `TextInput` — text input
- `ContactInput` — contact input
- `DateInput` — date selection (year / year-month / full date)
- `ListSelection` — list selection (`SingleSelect` / `MultiSelect`)

---

## Concepts at a glance

### Commands

A command processes a single `Update` and returns a `CommandResult`.  
Commands are created per update; dialog state lives in `ChatSession`.

- **Stateless**: no session persistence, good for `/help`, simple `/start`
- **Stateful**: stores `ChatSession`, good for dialogs and multi-step flows

### ChatSession

`ChatSession` represents current dialog state (current command, current step, step initialization flag).  
This is what lets TeleFlow resume conversations across updates.

### CommandResult & interpreters

Commands do not mutate the session directly.  
Instead, they return a `CommandResult`, and a chain of interpreters applies the actual behavior:
- exit command
- navigate to another command
- move between steps
- hold on current step (invalid input / retry / initialization)

### Interceptors

Interceptors are command-scoped (similar to ASP.NET filters):  
authorization, validation, logging, expected error handling — without touching global middleware.

---

## Documentation

Full docs: https://lamabucket.github.io/TeleFlow/

Suggested reading order:
1. Quick Start
2. Update pipeline & middleware
3. Command interpreters
4. Commands & chat session
5. Steps & step results
6. Interceptors

---

## Contributing

PRs are welcome:
1. Fork the repo
2. Create a branch
3. Commit changes
4. Open a PR

If you want to propose an API change, please open an issue first.

---

## Notes on stability

TeleFlow is actively evolving. If you rely on it in production, pin versions and review release notes.