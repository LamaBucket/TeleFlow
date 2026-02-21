using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class CommandStepRouterBuilder
{
    private readonly List<Func<ICommandStep>> _stepCommandFactories;

    public CommandStepRouterBuilder Add(Func<ICommandStep> factory, bool prepend = false)
    {
        _stepCommandFactories.Insert(prepend ? 0 : _stepCommandFactories.Count, factory);

        return this;
    }

    #region  Text
    
    public CommandStepRouterBuilder AddTextInput(TextInputCommandStepOptions options) 
        => Add(() => new TextInputCommandStep(options));

    public CommandStepRouterBuilder AddTextInput(string userPrompt, Func<CommandStepCommitContext, string, Task> onUserCommit) 
        => AddTextInput(new() { UserPrompt = userPrompt, OnUserCommit = onUserCommit });
    
    #endregion

    #region  Contact
    
    public CommandStepRouterBuilder AddContactInput(ContactInputCommandStepOptions options) 
        => Add(() => new ContactInputCommandStep(options));

    public CommandStepRouterBuilder AddContactInput(string userPrompt, Func<CommandStepCommitContext, Contact, Task> onUserCommit) 
        => AddContactInput(new() { UserPrompt = userPrompt, OnUserCommit =  onUserCommit});
    
    #endregion

    #region  List
    
    public CommandStepRouterBuilder AddListSelect<T>(ListSelectionOptions<T> options, CallbackCommandStepBaseOptions optionsBase)
        => Add(() => new ListSelectionCommandStep<T>(options, optionsBase));
    
    
    public CommandStepRouterBuilder AddSingleSelect<T>(string userPrompt, Func<IServiceProvider, Task<IReadOnlyList<T>>> valueProvider, Func<T, string> displayName, Func<CommandStepCommitContext, T, Task> onCommit)
        => AddListSelect<T>(
            options: new() 
            { 
                ValueProvider = valueProvider,
                Mode = new ListSelectionMode.SingleSelect<T>() { OnCommit = onCommit },
                DisplayNameParser = displayName
            }, 
            optionsBase: new() { UserPrompt = userPrompt }
        );

    public CommandStepRouterBuilder AddSingleSelect<T>(string userPrompt, IEnumerable<T> items, Func<T, string> displayName, Func<CommandStepCommitContext, T, Task> onCommit)
        => AddSingleSelect(
            valueProvider: async (sp) => items.ToList(),
            displayName: displayName,
            onCommit: onCommit,
            userPrompt: userPrompt
        );


    public CommandStepRouterBuilder AddMultiSelect<T>(string userPrompt, Func<IServiceProvider, Task<IReadOnlyList<T>>> valueProvider, Func<T, string> displayName, Func<CommandStepCommitContext, IReadOnlyList<T>, Task> onCommit)
        => AddListSelect<T>(
            options: new() 
            { 
                ValueProvider = valueProvider,
                Mode = new ListSelectionMode.MultiSelect<T>() { OnCommit = onCommit },
                DisplayNameParser = displayName
            }, 
            optionsBase: new() { UserPrompt = userPrompt }
        );

    public CommandStepRouterBuilder AddMultiSelect<T>(string userPrompt, IEnumerable<T> items, Func<T, string> displayName, Func<CommandStepCommitContext, IReadOnlyList<T>, Task> onCommit)
        => AddMultiSelect(
            valueProvider: async (sp) => items.ToList(),
            displayName: displayName,
            onCommit: onCommit,
            userPrompt: userPrompt
        );

    
    #endregion

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