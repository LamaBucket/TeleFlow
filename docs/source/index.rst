.. TeleFlowDocs documentation master file, created by
   sphinx-quickstart on Sun Feb 15 13:37:15 2026.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

TeleFlow
========

**TeleFlow** is a structured command & flow framework for building scalable Telegram bots in C#.

It extends `Telegram.Bot`_ with a predictable execution pipeline, per-chat session management, and composable multi-step commands — designed for developers who care about architecture, testability, and clean separation of concerns.

Architecture Overview
---------------------

TeleFlow is built around several core concepts:

**Update Handling Pipeline**
    Every ``Update`` from `Telegram.Bot`_ passes through a structured middleware pipeline with extensible points.  
    You can plug custom behaviors before, after, or around command execution.

**Per-Chat Session**
    Each chat has its own session state.  
    TeleFlow automatically restores the active command from the session store via middleware.

**State Isolated from Runtime**
    Commands do not live between updates.  
    All persistent data is stored explicitly in session stores, keeping runtime stateless and predictable.

**Composable Commands**
    TeleFlow supports both:

    - Stateless (single-step) commands  
    - Stateful (multi-step) flow commands  

    Flow commands are composed from reusable steps — like building blocks.

**Interceptors**
    Commands can define interceptors — pre-execution handlers that validate or transform incoming updates before the command logic runs.

When to Use TeleFlow
--------------------

TeleFlow is a good fit when:

- Your bot contains multi-step conversational flows
- You need reliable per-chat state restoration
- You want a middleware pipeline similar to ASP.NET Core
- You care about maintainable architecture instead of handler spaghetti

If you're building a minimal one-handler bot, plain `Telegram.Bot`_ may be enough.


.. toctree::

   :caption: Getting started

   getting-started/installation
   getting-started/first-bot

.. _Telegram.Bot: https://github.com/TelegramBots/Telegram.Bot
