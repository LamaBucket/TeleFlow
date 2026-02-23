Сессия чата
===========

``ChatSession`` представляет текущее состояние диалога
между пользователем и ботом.

Сессия используется для:

- определения активной команды;
- хранения номера текущего шага;
- передачи состояния между обновлениями.

Именно на основании ``ChatSession``
фабрика команд создаёт экземпляр команды
при каждом новом ``Update``.

---

Структура ChatSession
---------------------

Упрощённая структура класса:

.. code-block:: csharp

   public sealed class ChatSession
   {
       public string CurrentCommand { get; }
       public int CurrentCommandStep { get; }
       public bool IsStepInitialized { get; }

       public void GoToStep(int step);
       public void InitializeStep();

       public ChatSession(string currentCommand, int currentCommandStep = 0);
   }

Сессия хранит:

- имя текущей команды;
- номер текущего шага;
- флаг инициализации шага.

.. note::
    
    ``ChatSession`` не содержит бизнес-данных.
    Она описывает только состояние выполнения команды.

---

Stateless и Stateful команды
----------------------------

``ChatSession`` создаётся **для всех команд**.

Различие заключается в том,
сохраняется ли она после обработки обновления.

Stateless-команды
~~~~~~~~~~~~~~~~~

- используют ``ChatSession`` во время выполнения;
- не сохраняют её в хранилище;
- завершаются сразу после обработки.

Stateful-команды
~~~~~~~~~~~~~~~~

- используют ``ChatSession`` между обновлениями;
- сохраняют её в ``IChatSessionStore``;
- продолжают выполнение на следующем шаге.

.. important::

   Экземпляр команды не хранится между обновлениями.
   Сохраняется только ``ChatSession``.

---

ChatSessionStepSnapshot
-----------------------

В Stateful-командах шаги получают
``ChatSessionStepSnapshot``.

.. code:: csharp

    public readonly record struct ChatSessionStepSnapshot(int CurrentCommandStep, bool IsStepInitialized);

Snapshot содержит только:

- номер шага;
- флаг его инициализации.

Это гарантирует,
что шаг не изменяет ``ChatSession`` напрямую.

Изменение состояния происходит
через интерпретаторы ``CommandResult``.

---

IChatSessionStore
-----------------

``IChatSessionStore`` определяет механизм хранения сессии.

.. code-block:: csharp

   public interface IChatSessionStore
   {
       Task<ChatSession?> GetAsync(long chatId);
       Task SetAsync(long chatId, ChatSession session, TimeSpan? ttl = null);
       Task RemoveAsync(long chatId);
   }

Хранилище отвечает за:

- загрузку текущей сессии;
- сохранение состояния;
- удаление при завершении команды.

---

In-memory реализация
--------------------

В пакете ``TeleFlow.Extensions.DI`` доступна
встроенная реализация:

.. code-block:: csharp

   public class InMemoryChatSessionStore : IChatSessionStore
   {
       private readonly Dictionary<long, ChatSession> _sessions = [];

       public Task<ChatSession?> GetAsync(long chatId)
           => Task.FromResult(_sessions.GetValueOrDefault(chatId));

       public Task RemoveAsync(long chatId)
       {
           _sessions.Remove(chatId);
           return Task.CompletedTask;
       }

       public Task SetAsync(long chatId,
                            ChatSession session,
                            TimeSpan? ttl = null)
       {
           _sessions[chatId] = session;
           return Task.CompletedTask;
       }
   }

Эта реализация подходит для разработки
и однопроцессных приложений.

В production-сценариях рекомендуется использовать
распределённое хранилище (например, Redis).

---

Жизненный цикл сессии
---------------------

1. При получении ``Update`` загружается ``ChatSession``.
2. Команда выполняется.
3. Интерпретаторы принимают решение:
   - сохранить сессию;
   - обновить её;
   - удалить её.

.. important::
    
    Pipeline не изменяет ``ChatSession`` напрямую.
    Управление состоянием сосредоточено
    в логике интерпретаторов.