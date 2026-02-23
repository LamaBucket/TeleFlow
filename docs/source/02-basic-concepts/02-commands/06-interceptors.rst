Интерсепторы
============

Введение
--------

Interceptor — механизм расширения поведения команды.

Он позволяет выполнить дополнительную логику
до выполнения команды и при необходимости
заменить её результат.

В отличие от middleware,
интерсепторы работают только на уровне конкретной команды.
Они не являются частью глобального Pipeline.

.. note::

   По своей роли Interceptor ближе к фильтрам (Filters) в ASP.NET.
   Он расширяет поведение конкретной команды,
   тогда как middleware влияет на весь Pipeline.

---

Жизненный цикл
--------------

Во время обработки ``Update`` последовательность выглядит так:

1. Создаётся экземпляр команды.
2. Выполняется цепочка Interceptor.
3. Если ни один Interceptor не вернул ``CommandResult``,
   выполняется логика команды.
4. Возвращённый результат передаётся интерпретаторам.

Если Interceptor возвращает ``CommandResult``,
выполнение команды не происходит.

---

Назначение Interceptor
----------------------

Interceptor связан с конкретной командой
и применяется только к ней.

Он подходит для задач, которые:

- относятся к определённой команде;
- не требуют изменения глобального Pipeline;
- должны быть централизованы для этой команды.

Типичные сценарии:

- проверка авторизации;
- ограничение доступа;
- валидация входных данных;
- логирование выполнения команды;
- обработка ожидаемых ошибок.

---

Интерфейс
---------

Пример интерфейса:

.. code-block:: csharp

   public interface ICommandInterceptor
   {
       Task<CommandResult?> Intercept(UpdateContext update);
   }

Метод ``Intercept``:

- получает ``UpdateContext``;
- возвращает ``CommandResult`` для прерывания выполнения;
- возвращает ``null``, если выполнение должно продолжиться.

---

Пример Interceptor
------------------

.. code-block:: csharp

   public class TextValidationInterceptor : ICommandInterceptor
   {
       public async Task<CommandResult?> Intercept(UpdateContext update)
       {
           var text = update.Update.Message?.Text;

           if (text is not null && !text.StartsWith("PREFIX:"))
           {
               return new HoldOnStatefulResult(
                   HoldOnReason.InvalidInput,
                   "No prefix was used in your message!");
           }

           return null;
       }
   }

В этом примере:

- если условие не выполнено,
  возвращается ``CommandResult``;
- команда не запускается;
- управление переходит к интерпретаторам.

Если возвращено ``null``,
``UpdateContext`` передаётся в команду.

---

Регистрация
-----------

Интерсептор регистрируется вместе с командой:

.. code-block:: csharp

   commands.AddMultiStep("/start", steps =>
   {
       steps
           .AddTextInput("Hello! Please tell us your name!",
                         async (sp, name) => Console.WriteLine(name))
           .AddContactInput("Please send us your contact",
                            async (sp, contact) => Console.WriteLine(contact.PhoneNumber));
   })
   .AddInterceptor(() => new TextValidationInterceptor());

В этом случае Interceptor применяется только к команде ``/start``.

---

Когда использовать Interceptor
-------------------------------

Используйте Interceptor, если:

- логика относится к конкретной команде;
- требуется прервать выполнение до запуска команды;
- нужно централизовать проверку для этой команды.

Если логика должна применяться ко всем апдейтам,
следует использовать middleware.

---

Разграничение ответственности
-----------------------------

- Middleware — инфраструктурный уровень (вся система).
- Interceptor — уровень конкретной команды.
- Command — бизнес-логика.
- Interpreters — управление состоянием после выполнения.

Такое разделение делает архитектуру предсказуемой
и упрощает расширение системы.