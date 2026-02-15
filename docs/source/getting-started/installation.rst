Installation
============

Packages
--------
TeleFlow is split into multiple NuGet packages to keep the core modular and allow advanced setups.

Core building blocks
--------------------
**TeleFlow**
    The meta/manifest package that references the standard set of TeleFlow components.
    
**TeleFlow.Abstractions**
    Public contracts and interfaces used across the ecosystem.

**TeleFlow.Core**
    The runtime core: default middleware pipeline, command infrastructure, interceptors,
    and built-in internal services.

**TeleFlow.Extensions.DependencyInjection**
    Dependency Injection integration for Microsoft.Extensions.DependencyInjection.
    Provides the standard ``services.AddTeleFlow()`` entry point.

**TeleFlow.Telegram** *(planned)*
    A compatibility layer that integrates TeleFlow with ``Telegram.Bot`` while isolating
    breaking changes behind a dedicated package.


.. tip:: 

    For most users, you only need the **TeleFlow** package.

Install from NuGet
------------------

.. code:: zsh
    
    dotnet add package TeleFlow --version 2.0.0