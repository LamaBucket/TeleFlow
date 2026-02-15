Your First Bot
==============

This guide builds a minimal bot that:

- starts a multi-step flow on ``/start``
- asks the user to enter any text
- echoes the input back
- navigates back to ``/start`` to repeat

Prerequisites
-------------

- .NET installed
- A Telegram bot token (create a bot via BotFather)
- The **TeleFlow** NuGet package installed

Install
-------

.. code-block:: bash

   dotnet add package TeleFlow

Minimal setup
-------------

Configure your bot token using an environment variable:

.. code-block:: bash

   export TELEGRAM_BOT_TOKEN="PUT_YOUR_TOKEN_HERE"

Then create ``Program.cs``:

.. code-block:: csharp

   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Hosting;
   using TeleFlow.DI;
   using TeleFlow.Abstractions.Transport.Messaging;
   using TeleFlow.Abstractions.Engine.Commands.Results;

   var builder = Host.CreateApplicationBuilder(args);

   var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
   if (string.IsNullOrWhiteSpace(token))
       throw new InvalidOperationException("TELEGRAM_BOT_TOKEN is not set.");

   builder.Services.AddTeleFlow(options =>
   {
       options.ConfigureCommandRegistry(registry =>
       {
           registry.AddMultiStep("/start",
               stepBuilder =>
               {
                   stepBuilder.AddTextInput(
                       "Please enter something",
                       async (sp, input) =>
                       {
                           var messageService = sp.GetRequiredService<IMessageSender>();
                           await messageService.SendMessage(input);
                       });
               },
               onCompleted: async sp => new NavigateCommandResult("/start")
           ).EnableNavigation();
       });
   });

   var app = builder.Build();

   // Start polling / update processing (depends on your Telegram integration package setup).
   // If you already have TeleFlow.Telegram (or your current integration entry point),
   // call it here.

   await app.RunAsync();

Run
---

Start the app and send ``/start`` to your bot.  
The bot will prompt you for input and echo it back.

What’s next
-----------

- Learn how **commands** and **flows** are structured
- Add middleware and interceptors
- Use built-in interactive steps (buttons, pickers, etc.)
