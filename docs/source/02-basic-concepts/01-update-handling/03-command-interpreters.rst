Command Interpreters
====================

После выполнения команды Pipeline получает ``CommandResultContext``.
Дальнейшее поведение системы определяется интерпретаторами.

Интерпретаторы — это middleware,
работающие с ``CommandResultContext``.

Именно они определяют:

- какие типы ``CommandResult`` поддерживаются в системе;
- как обрабатывается каждый результат;
- каким образом завершается или продолжается выполнение.

---

Результаты выполнения команд
----------------------------

Все результаты наследуются от базового класса:

.. code-block:: csharp

   public abstract class CommandResult
   {
       public static CommandResult Exit => new ExitCommandResult();
   }

В TeleFlow доступны следующие типы результатов:

ExitCommandResult
~~~~~~~~~~~~~~~~~

Завершает текущую команду.

Сессия пользователя удаляется,
обработка продолжается без активной команды.

---

NavigateCommandResult
~~~~~~~~~~~~~~~~~~~~~

Инициирует переход к другой команде.

.. code-block:: csharp

   public class NavigateCommandResult : CommandResult
   {
       public string CommandToNavigate { get; init; }
       public NavigateCommandParameters Parameters { get; init; }
   }

Содержит:

- имя команды для перехода;
- параметры навигации.

---

GoToStatefulResult
~~~~~~~~~~~~~~~~~~

Используется внутри Stateful-команд
для перехода к другому шагу.

.. code-block:: csharp

   public class GoToStatefulResult : CommandResult
   {
       public int GoToStepNumber { get; init; }
       public bool InitializeNextStep { get; init; }
   }

Позволяет:

- установить номер шага;
- указать, требуется ли его инициализация.

---

HoldOnStatefulResult
~~~~~~~~~~~~~~~~~~~~

Оставляет выполнение на текущем шаге.

.. code-block:: csharp

   public class HoldOnStatefulResult : CommandResult
   {
       public HoldOnReason Reason { get; init; }
       public string? HoldOnMessage { get; init; }
   }

Используется, когда:

- ввод пользователя некорректен;
- требуется повторный ввод;
- шаг только инициализируется.

.. note::

    ``HoldOn`` не изменяет состояние шага
    и не завершает команду.

---

Цепочка интерпретаторов
-----------------------

Интерпретаторы образуют отдельную цепочку middleware,
которая обрабатывает ``CommandResultContext``.

Упрощённая схема:

.. digraph:: interpreter_chain
   :align: center

   "CommandResultContext"
      -> "ExitInterpreter"
      -> "NavigateInterpreter"
      -> "StatefulInterpreter"
      -> "DefaultCommandInterpreter";

Если ни один интерпретатор не обработал результат,
управление передаётся ``DefaultCommandInterpreter``.

---

DefaultCommandInterpreter
-------------------------

.. code-block:: csharp

   public class DefaultCommandInterpreter 
       : IHandler<CommandResultContext>
   {
       private readonly IChatSessionStore _sessionStore;

       public async Task Handle(CommandResultContext args)
       {
           var chatId = args.GetService<IChatIdProvider>()
                            .GetChatId();

           await _sessionStore.RemoveAsync(chatId);

           throw new Exception(
               "No command interpreter matched the command result of type "
               + args.CommandResult.GetType().FullName);
       }
   }

Этот обработчик:

- удаляет сессию;
- сигнализирует об отсутствии интерпретатора
  для данного типа результата.

---

Навигационный интерпретатор
---------------------------

``NavigateCommandResult`` обрабатывается
навигационным интерпретатором.

Его задача — выполнить переход к другой команде
внутри текущего цикла обработки.

Алгоритм работы:

1. Завершает текущую команду (очищает сессию).
2. Создаёт новую ``ChatSession``.
3. Создаёт целевую команду через ``ICommandFactory``.
4. Запускает её с тем же ``Update``.
5. Передаёт полученный результат во внутреннюю цепочку интерпретаторов.

Схема:

.. digraph:: navigate_flow
   :align: center

   "NavigateCommandResult"
      -> "Remove old session"
      -> "Create new session"
      -> "Execute target command"
      -> "Process result via inner interpreter chain";

Фактически создаётся локальный цикл обработки,
встроенный в текущий Pipeline.

---

EnableNavigation
----------------

Чтобы команда могла быть целью навигации,
её необходимо явно зарегистрировать:

.. code-block:: csharp

   commands
       .AddSendText("/start", "Hello, World!")
       .EnableNavigation();

Навигация создаётся не через входящий ``Update``,
поэтому команда должна быть доступна
в навигационной фабрике.

Параметры можно передать через ``EnableNavigation``
с использованием функции seed.

---

Navigate Depth
--------------

Навигация ограничивается глубиной,
чтобы избежать рекурсивных переходов.

Конфигурация:

.. code-block:: csharp

   builder.WithNavigateInterpreter(3);

По умолчанию глубина равна 3.

Технически это реализуется созданием вложенных
цепочек интерпретаторов.

Если глубина равна 0,
навигационный интерпретатор
не добавляется во внутреннюю цепочку.

---

Конфигурация интерпретаторов
----------------------------

Интерпретаторы настраиваются через
``InterpreterPipelineBuilder``.

Пример:

.. code-block:: csharp

   builder.WithNavigateInterpreter(3);

Можно добавлять собственные интерпретаторы
для обработки пользовательских типов ``CommandResult``.

---

Когда писать собственный интерпретатор
---------------------------------------

Собственный интерпретатор требуется, если:

- добавляется новый тип ``CommandResult``;
- изменяется семантика существующего результата;
- требуется специфическая логика завершения команды.

Интерпретатор должен обрабатывать конкретный тип
``CommandResult`` и быть включён в цепочку.