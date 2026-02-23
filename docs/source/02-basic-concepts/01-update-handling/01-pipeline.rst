Pipeline обработки
==================

В TeleFlow обработка обновлений реализована как цепочка обработчиков
(Handler chain).

Каждый входящий ``Update`` проходит через фиксированную последовательность
этапов. Эта последовательность называется Pipeline.

Pipeline определяет порядок обработки обновления,
момент создания и расширения контекста,
а также точку, в которой выполняется команда и обрабатывается её результат.

В большинстве случаев структура Pipeline остаётся неизменной.
Обычно пользователь добавляет только собственные middleware
на уже существующие этапы обработки.

---

Архитектурная модель
--------------------

Pipeline построен на базовом интерфейсе обработчика:

.. code-block:: csharp

   public interface IHandler<T>
   {
       Task Handle(T args);
   }

Каждый обработчик принимает объект типа ``T`` и выполняет работу
над этим аргументом.

Middleware расширяет эту модель:

.. code-block:: csharp

   public interface IHandlerMiddleware<T, TNext> : IHandler<T>
   {
       IHandler<TNext> Next { get; }
   }

   public interface IHandlerMiddleware<T>
       : IHandlerMiddleware<T, T>
   { }

Middleware хранит ссылку на следующий обработчик в цепочке
и передаёт управление через ``Next.Handle(...)``.

Таким образом Pipeline представляет собой последовательность
взаимосвязанных обработчиков.

---

Общая схема обработки
---------------------

По умолчанию один ``Update`` проходит через следующие этапы:

.. digraph:: update_context_flow
   :align: center

   "Update" -> "UpdateContext" -> "SessionContext" -> "CommandResultContext";

Каждый переход реализуется соответствующим middleware.

---

Контекст выполнения
-------------------

Pipeline не изменяет сам ``Update``.
Вместо этого он последовательно оборачивает его в новые объекты контекста.

Update
~~~~~~

Исходный объект Telegram.Bot.
На этом этапе доступны только входящие данные.

---

UpdateContext
-------------

.. code-block:: csharp

   public class UpdateContext
   {
       public Update Update { get; init; }

       public IServiceProvider ServiceProvider { get; init; }

       public TService GetService<TService>()
       {
           return ServiceProvider.GetService<TService>()
               ?? throw new InvalidOperationException();
       }

       public UpdateContext(Update update, IServiceProvider serviceProvider)
       {
           Update = update;
           ServiceProvider = serviceProvider;
       }
   }

``UpdateContext`` добавляет инфраструктурный контекст выполнения.

На этой стадии:

- уже известен ``chatId``;
- создаётся scope зависимостей;
- становятся доступны ``scoped`` сервисы.

Этот объект является базой для всех последующих стадий.

---

SessionContext
--------------

.. code-block:: csharp

   public class SessionContext : UpdateContext
   {
       public ChatSession Session { get; init; }

       public SessionContext(ChatSession session, UpdateContext context)
           : this(session, context.Update, context.ServiceProvider)
       { }

       public SessionContext(ChatSession session,
                             Update update,
                             IServiceProvider serviceProvider)
           : base(update, serviceProvider)
       {
           Session = session;
       }
   }

``SessionContext`` расширяет ``UpdateContext`` и добавляет ``ChatSession``.

На этой стадии:

- восстанавливается текущая сессия пользователя;
- определяется активная команда;
- становится доступным состояние диалога.

---

CommandResultContext
--------------------

.. code-block:: csharp

   public class CommandResultContext<TCommandResult> : SessionContext
       where TCommandResult : CommandResult
   {
       public TCommandResult CommandResult { get; init; }

       public CommandResultContext(
           TCommandResult commandResult,
           SessionContext sessionContext)
           : this(commandResult,
                  sessionContext.Session,
                  sessionContext)
       { }

       public CommandResultContext(
           TCommandResult commandResult,
           ChatSession session,
           UpdateContext context)
           : base(session, context)
       {
           CommandResult = commandResult;
       }
   }

После выполнения команды создаётся ``CommandResultContext``.

Он добавляет результат выполнения команды и используется
для финальной стадии обработки.

.. note::

   Именно на стадии интерпретации ``CommandResultContext``
   принимается решение о сохранении или удалении текущей сессии пользователя.

---

Границы ответственности
-----------------------

Pipeline отвечает за:

- порядок обработки;
- создание и расширение контекста;
- передачу управления между обработчиками.

Pipeline не содержит бизнес-логики.
Бизнес-логика располагается внутри команд и шагов.

Пользователь библиотеки не создаёт обработчики вручную,
но может добавлять собственные middleware
в существующую цепочку.