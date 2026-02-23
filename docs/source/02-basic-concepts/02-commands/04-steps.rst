Шаги
====

Введение
--------

Шаги (Step) — основа многошаговых (Stateful) команд в TeleFlow.

Stateful-команда строится как последовательность шагов.
Каждый шаг отвечает за один понятный участок диалога:
запросить данные, принять ввод, проверить его и решить,
что делать дальше.

Пример многошаговой команды:

.. code-block:: csharp

   services.AddTeleFlowPolling(
       botToken: token,
       teleFlow: options =>
       {
           options.ConfigureCommandRouters(commands =>
           {
               commands.AddMultiStep("/start", steps =>
               {
                   steps
                       .AddTextInput(
                           "Hello! Please tell us your name!",
                           async (sp, name) => Console.WriteLine(name))
                       .AddContactInput(
                           "Please send us your contact",
                           async (sp, contact) => Console.WriteLine(contact.PhoneNumber));
               });
           });
       });

Шаги можно комбинировать в любом порядке.
Набор базовых шагов расширяется со временем.

---

Базовые шаги
------------

TeleFlow включает готовые шаги, которые можно использовать без написания собственного кода:

- ``TextInput`` — ввод текста.
- ``ContactInput`` — получение контакта.
- ``DateInput`` — выбор даты (год / год-месяц / год-месяц-день).
- ``ListSelection`` — выбор из списка (``SingleSelect`` / ``MultiSelect``).

.. note::

   ``ContactInput`` принимает любой отправленный контакт.
   Если нужно проверять, принадлежит ли контакт текущему пользователю,
   это обычно делается через Interceptor (см. :doc:`06-interceptors`).

---

Как устроен шаг
---------------

Каждый шаг реализует ``ICommandStep``:

.. code-block:: csharp

   public interface ICommandStep
   {
       Task<CommandStepResult> Handle(UpdateContext args);
       Task OnEnter(IServiceProvider serviceProvider);
   }

Шаг состоит из двух частей:

- ``OnEnter`` — вызывается при входе в шаг.
  Обычно здесь отправляется prompt пользователю и подготавливается состояние шага.
- ``Handle`` — вызывается на следующих обновлениях и обрабатывает ввод пользователя.
  Возвращает ``CommandStepResult``, который описывает дальнейшее действие.

.. important::

   ``CommandStepResult`` и ``CommandResult`` — разные вещи.

   ``CommandStepResult`` — результат работы шага.
   Он преобразуется в ``CommandResult`` внутри оркестратора Stateful-команды.

Подробно о ``CommandStepResult`` → :doc:`05-step-result`.

---

Stateful и Stateless шаги
-------------------------

Шаги могут быть одноразовыми или длительными.

Одноразовые (stateless)
~~~~~~~~~~~~~~~~~~~~~~~

Завершаются после первого валидного ввода.
Не хранят внутреннее состояние между обновлениями.

Длительные (stateful)
~~~~~~~~~~~~~~~~~~~~~

Хранят состояние и могут требовать несколько валидных действий,
прежде чем завершиться.

.. note::

   Независимо от типа, шаг не завершится, пока не получит валидный ввод.
   "Stateless" не означает "живёт один Update".

---

StepState
---------

Stateful шаги хранят своё состояние в ``StepState<TVm>``:

.. code-block:: csharp

   public sealed class StepState<TVm> where TVm : class
   {
       public int MessageId { get; init; }
       public TVm ViewModel { get; init; }

       public StepState(int messageId, TVm viewModel)
       {
           MessageId = messageId;
           ViewModel = viewModel;
       }
   }

- ``MessageId`` — идентификатор сообщения, которое инициировало состояние шага.
- ``ViewModel`` — данные, необходимые шагу для дальнейшей работы.

---

Шаги на inline-кнопках
----------------------

Некоторые шаги работают на inline-кнопках и требуют состояния,
потому что пользователь взаимодействует не текстом, а callback-данными.

На текущий момент это:

- ``ListSelection``
- ``DateInput``

Для таких шагов существует базовый класс
``CallbackCommandStepBase<TVm>``.
Он упрощает обработку callback-обновлений и работу со состоянием.

Подробности → раздел Advanced.

---

Оркестратор шагов
-----------------

В TeleFlow Stateful-команды выполняются через встроенный оркестратор.
Он отвечает за:

- выбор текущего шага по данным ``ChatSession``;
- вызов ``OnEnter`` при входе в шаг;
- выполнение ``Handle``;
- преобразование ``CommandStepResult`` в ``CommandResult``;
- переходы между шагами и завершение команды.

Код оркестратора обычно не нужен при использовании готовых шагов,
поэтому он не приводится на этой странице.

---

Связанные разделы
-----------------

- Сессия чата и номер текущего шага → :doc:`02-chat-session`
- Результаты команды → :doc:`03-command-result`
- Результаты шага → :doc:`05-step-result`