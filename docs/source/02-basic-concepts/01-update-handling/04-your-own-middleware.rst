Writing Custom Middleware
=========================

Введение
--------

В большинстве случаев стандартного Pipeline достаточно.
Однако TeleFlow позволяет добавлять собственные middleware
в существующую цепочку обработки.

В этом разделе рассматривается создание контекстного middleware.
Переходные middleware относятся к расширенной конфигурации
и рассматриваются как advanced-сценарий.

---

Когда нужен собственный middleware
-----------------------------------

Middleware применяется глобально —
он выполняется для всех апдейтов
на соответствующей стадии Pipeline.

Собственный middleware имеет смысл писать, если требуется:

- добавить поддержку нового типа ``CommandResult`` и создать интерпретатор;
- логировать входящие апдейты;
- реализовать глобальную обработку ошибок;
- измерять время выполнения;
- реализовать инфраструктурное поведение, не связанное с конкретной командой.

Если логика относится к одной конкретной команде,
следует использовать Interceptor.

---

Создание middleware
-------------------

Для создания собственного middleware
необходимо реализовать интерфейс ``IHandlerMiddleware<T>``.

Пример middleware, работающего на стадии ``UpdateContext``:

.. code-block:: csharp

    using Microsoft.Extensions.Logging;
    using TeleFlow.Abstractions.Engine.Pipeline;
    using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

    public class LoggingMiddleware : IHandlerMiddleware<UpdateContext>
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        public IHandler<UpdateContext> Next { get; }

        public LoggingMiddleware(
            ILogger<LoggingMiddleware> logger,
            IHandler<UpdateContext> next)
        {
            _logger = logger;
            Next = next;
        }

        public async Task Handle(UpdateContext context)
        {
            if (context.Update.Message?.Text is not null)
            {
                var chatId = context.Update.Message.Chat.Id;
                var text   = context.Update.Message.Text;

                _logger.LogInformation(
                    "New message in chat {ChatId}: {Message}",
                    chatId,
                    text);
            }

            await Next.Handle(context);
        }
    }

Этот middleware будет логировать все входящие текстовые сообщения
на стадии ``UpdateContext``.

.. important::

   Передача управления следующему обработчику
   выполняется через ``Next.Handle(context)``.

   Если не вызвать ``Next``,
   дальнейшая обработка Pipeline будет остановлена.

---

Регистрация middleware
----------------------

Чтобы подключить middleware,
его необходимо добавить в конфигурацию TeleFlow.

Пример:

.. code-block:: csharp

    var builder = Host.CreateDefaultBuilder();

    builder
        .ConfigureServices(services =>
            services.AddTeleFlowPolling(
                botToken: token,
                teleFlow: options =>
                {
                    options.ConfigureMiddlewarePipeline(middleware =>
                    {
                        middleware
                            .WithUpdateContextMiddleware<LoggingMiddleware>();
                    });
                }));

    await builder.Build().RunAsync();

Порядок регистрации определяет порядок выполнения.

---

Результат работы
----------------

После регистрации middleware
при каждом входящем сообщении
в журнале будет появляться запись:

.. code-block:: text

    [Information] New message in chat 123456789: Hello

(сюда можно добавить скриншот лога)

---

Практические рекомендации
--------------------------

- Middleware должен быть максимально лёгким.
- Не следует помещать в него бизнес-логику.
- Не рекомендуется выполнять долгие блокирующие операции.
- Следует учитывать порядок выполнения при регистрации нескольких middleware.