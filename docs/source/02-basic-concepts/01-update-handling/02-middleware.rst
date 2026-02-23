Middleware
==========

Введение
--------

Pipeline TeleFlow реализован как цепочка ``IHandler<T>``.
Каждое звено этой цепочки является middleware.

Middleware получает объект контекста,
выполняет обработку и передаёт управление следующему звену.

.. note:: 
    
    Большинство пользовательских расширений Pipeline
    реализуются именно через добавление собственных middleware.

---

Типы middleware
---------------

В Pipeline можно выделить два типа middleware.

Контекстные middleware
~~~~~~~~~~~~~~~~~~~~~~

Работают с уже существующим типом контекста
и не изменяют его тип.

Например:

- ``IHandlerMiddleware<UpdateContext>``
- ``IHandlerMiddleware<SessionContext>``
- ``IHandlerMiddleware<CommandResultContext>``

Это естественная точка расширения Pipeline.
Именно здесь обычно добавляется пользовательская логика.

---

Переходные middleware
~~~~~~~~~~~~~~~~~~~~~

Преобразуют один тип контекста в другой.

Например:

- ``Update → UpdateContext``
- ``UpdateContext → SessionContext``
- ``SessionContext → CommandResultContext``

Эти middleware формируют структуру Pipeline.

.. note::

   В большинстве случаев переходные middleware не изменяются.
   При необходимости их можно переопределить и встроить
   в собственную конфигурацию Pipeline.

---

Интерпретаторы
--------------

Middleware, работающие с ``CommandResultContext``,
образуют цепочку интерпретаторов.

Именно они определяют,
как обрабатывается результат выполнения команды.

В конце цепочки интерпретаторов всегда находится
``DefaultCommandInterpreter``:

.. code-block:: csharp

   public class DefaultCommandInterpreter : IHandler<CommandResultContext>
   {
       private readonly IChatSessionStore _sessionStore;

       public async Task Handle(CommandResultContext args)
       {
           var chatId = args.GetService<IChatIdProvider>().GetChatId();

           await _sessionStore.RemoveAsync(chatId);

           throw new Exception(
               "No command interpreter matched the command result of type "
               + args.CommandResult.GetType().FullName);
       }

       public DefaultCommandInterpreter(IChatSessionStore sessionStore)
       {
           _sessionStore = sessionStore;
       }
   }

Этот обработчик выполняется,
если ни один интерпретатор не обработал ``CommandResult``.

---

Порядок выполнения
------------------

Middleware выполняются строго в порядке их регистрации.

Каждый middleware:

1. Получает контекст.
2. Выполняет свою логику.
3. Передаёт управление через ``Next.Handle(...)``.

Если управление не передаётся дальше,
цепочка на текущем этапе завершается.

---

Когда писать собственный middleware
------------------------------------

Собственный middleware имеет смысл добавлять,
если требуется:

- глобальное логирование;
- централизованная обработка ошибок;
- изменение инфраструктурного поведения;
- дополнительная обработка контекста до или после команды.

Если логика относится к конкретной команде,
следует использовать Interceptor.