Первый бот
==========

Введение
--------

На этой странице мы соберём минимального conversation-бота на TeleFlow.

Сценарий (мини-FSM):

1) команда ``/start`` → приветствие  
2) команда ``/test`` → бот спросит имя → предложит оценить библиотеку (1..5) → отправит итоговое сообщение

Цель — показать главную идею TeleFlow: бот собирается из готовых блоков, а обработка апдейтов уже организована через pipeline.

---

Что получится в итоге
---------------------

После ``/test`` пользователь увидит:

- сообщение с просьбой ввести имя
- затем inline-выбор оценки (1..5)
- затем итог: ``Your name is ... and you rated us ... star(s)!``

---

Код целиком
-----------

.. code-block:: csharp

   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Hosting;
   using Microsoft.Extensions.Logging;

   using TeleFlow.Extensions.DI.Polling;

   using TeleFlow.Abstractions.Engine.ChatIdentity;
   using TeleFlow.Abstractions.Engine.Commands.Results;
   using TeleFlow.Abstractions.Transport.Messaging;

   var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
   if (string.IsNullOrWhiteSpace(token))
       throw new InvalidOperationException("Переменная окружения TELEGRAM_BOT_TOKEN не задана.");

   // ВНИМАНИЕ: это демо in-memory хранения (для первого примера).
   // В production лучше использовать ChatSessionStore / свою БД / кэш.
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
                       // Финализация выполняется в scope текущего апдейта.
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

---

Разбор: что здесь происходит
----------------------------

1) ``AddTeleFlowPolling(...)``  
   Подключает TeleFlow в режиме Polling и регистрирует нужные сервисы.

2) ``ConfigureCommandRouters(...)``  
   Здесь мы описываем команды как набор готовых блоков.

3) ``AddSendText("/start", ...)``  
   Самая простая stateless-команда: на входящий ``/start`` отправляем текст.

4) ``AddMultiStep("/test", ...)``  
   Многошаговая команда: TeleFlow хранит состояние диалога и двигает пользователя по шагам.

5) ``AddTextInput(...)``  
   Шаг, который ждёт текстовое сообщение и извлекает значение.

6) ``AddSingleSelect(...)``  
   Шаг с выбором одного значения (inline-кнопки).

7) ``onCompleted``  
   Вызывается, когда шаги завершены. Здесь мы формируем итог и отправляем сообщение.

---

Почему это "FSM"
----------------

В Telegram.Bot вы бы вручную:

- отличали "ввод имени" от "ввода оценки"
- хранили состояние (в памяти/БД)
- маршрутизировали апдейты по этому состоянию

В TeleFlow состояние диалога и маршрутизация уже заложены в модель команды/шагов.

---
