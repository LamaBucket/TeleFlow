Установка
==========

Введение
--------

На этой странице описано:

- требования к окружению
- установка TeleFlow через NuGet
- базовая настройка токена
- выбор способа получения обновлений (Polling / Webhook)
- регистрация TeleFlow в DI
- минимальный запуск приложения

После выполнения этих шагов вы получите минимально работающего бота.

---

Требования
----------

Перед установкой убедитесь, что у вас есть:

- .NET 8 SDK (рекомендуемая версия)
- Telegram Bot Token (получается через @BotFather)
- (опционально) HTTPS-домен для Webhook режима

Проверить установленную версию .NET:

.. code-block:: bash

   dotnet --version

---

Установка через NuGet
----------------------

Создайте новый проект:

.. code-block:: bash

   dotnet new console -n MyBot
   cd MyBot

Добавьте пакет TeleFlow:

.. code-block:: bash

   dotnet add package TeleFlow

Проверьте, что пакет добавлен:

.. code-block:: bash

   dotnet list package

---

Настройка токена
----------------

Рекомендуемый способ — использовать переменную окружения.

Linux / macOS:

.. code-block:: bash

   export TELEGRAM_BOT_TOKEN="ВАШ_ТОКЕН"

Windows (PowerShell):

.. code-block:: powershell

   setx TELEGRAM_BOT_TOKEN "ВАШ_ТОКЕН"

---

Минимальная конфигурация (Polling)
-----------------------------------

Создайте `Program.cs`:

.. code-block:: csharp

   using Microsoft.Extensions.Hosting;
   using Microsoft.Extensions.DependencyInjection;

   var builder = Host.CreateApplicationBuilder(args);

   var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

   if (string.IsNullOrWhiteSpace(token))
       throw new InvalidOperationException("Переменная TELEGRAM_BOT_TOKEN не задана.");

   builder.Services.AddTeleFlow(options =>
   {
       options.BotToken = token;
       options.UsePolling();
   });

   var app = builder.Build();
   app.Run();

После запуска бот начнет получать обновления через long polling.

---

Webhook режим
-------------

Webhook используется для production-окружения и требует HTTPS.

Пример конфигурации:

.. code-block:: csharp

   builder.Services.AddTeleFlow(options =>
   {
       options.BotToken = token;
       options.UseWebhook(webhook =>
       {
           webhook.Url = "https://your-domain.com/bot";
           webhook.Path = "/bot";
       });
   });

Важно:

- HTTPS обязателен
- требуется валидный SSL-сертификат
- порт должен быть доступен извне

---

Polling vs Webhook
------------------

**Polling**

- проще для разработки
- не требует HTTPS
- подходит для локального запуска

**Webhook**

- подходит для production
- Telegram отправляет обновления напрямую
- ниже задержка
- лучше масштабируется

---

Регистрация в DI
-----------------

TeleFlow полностью интегрируется с `Microsoft.Extensions.DependencyInjection`.

Минимальная регистрация:

.. code-block:: csharp

   builder.Services.AddTeleFlow(options =>
   {
       options.BotToken = token;
   });

Дополнительно можно:

- регистрировать свои команды
- подключать Interceptors
- заменять middleware
- настраивать хранилище сессий

---

Проверка работы
---------------

После запуска:

1. Напишите боту `/start`
2. Убедитесь, что приложение не падает
3. Проверьте логи — TeleFlow сообщает о запуске транспорта

Для webhook можно проверить:

.. code-block:: bash

   curl https://api.telegram.org/bot<ТОКЕН>/getWebhookInfo

---

Типичные проблемы
------------------

**TELEGRAM_BOT_TOKEN не задан**

→ Проверьте переменные окружения.

**Webhook не работает**

→ Проверьте:
   - HTTPS
   - валидность сертификата
   - открытые порты
   - правильность URL

**Пакет не устанавливается**

→ Убедитесь, что используется .NET 8  
→ Выполните `dotnet restore`


