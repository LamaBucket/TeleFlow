using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DependencyInjection.Builders.Commands.Stateful;

public class CommandStepRouterBuilder
{
    private readonly List<Func<ICommandStep>> _stepCommandFactories;

    public CommandStepRouterBuilder Add(Func<ICommandStep> factory, bool prepend = false)
    {
        _stepCommandFactories.Insert(prepend ? 0 : _stepCommandFactories.Count, factory);

        return this;
    }

    public CommandStepRouterBuilder AddTextInput(TextInputCommandStepOptions options) 
        => Add(() => new TextInputCommandStep(options));

    public CommandStepRouterBuilder AddTextInput(string userPrompt, Func<CommandStepCommitContext, string, Task> onUserCommit) 
        => AddTextInput(new() { UserPrompt = userPrompt, OnUserCommit = onUserCommit });


    public CommandStepRouterBuilder AddContactInput(ContactInputCommandStepOptions options) 
        => Add(() => new ContactInputCommandStep(options));

    public CommandStepRouterBuilder AddContactInput(string userPrompt, Func<CommandStepCommitContext, Contact, Task> onUserCommit) 
        => AddContactInput(new() { UserPrompt = userPrompt, OnUserCommit =  onUserCommit});


    public CommandStepRouter Build()
    {
        if(_stepCommandFactories.Count == 0)
            throw new Exception("No Step Commands Provided!");

        return new CommandStepRouter([.. _stepCommandFactories]);
    }

    public CommandStepRouterBuilder()
    {
        _stepCommandFactories = [];
    }
}