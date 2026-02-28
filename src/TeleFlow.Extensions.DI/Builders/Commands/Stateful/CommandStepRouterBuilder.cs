using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Configuration;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class CommandStepRouterBuilder
{
    private readonly List<StepDescriptor> _stepDescriptors;


    public CommandStepFilterBuilder Add(Func<ICommandStep> factory, bool prepend = false)
    {
        StepDescriptor descriptor = new(factory);

        _stepDescriptors.Insert(prepend ? 0 : _stepDescriptors.Count, descriptor);

        return new CommandStepFilterBuilder(descriptor, this);
    }

    #region  Text
    
    public CommandStepFilterBuilder AddTextInput(TextInputCommandStepOptions options) 
        => Add(() => new TextInputCommandStep(options));

    public CommandStepFilterBuilder AddTextInput(string userPrompt, Func<CommandStepCommitContext, string, Task> onUserCommit) 
        => AddTextInput(new() { UserPrompt = userPrompt, OnUserCommit = onUserCommit });
    
    #endregion

    #region  Contact
    
    public CommandStepFilterBuilder AddContactInput(ContactInputCommandStepOptions options) 
        => Add(() => new ContactInputCommandStep(options));

    public CommandStepFilterBuilder AddContactInput(string userPrompt, Func<CommandStepCommitContext, Contact, Task> onUserCommit) 
        => AddContactInput(new() { UserPrompt = userPrompt, OnUserCommit =  onUserCommit});
    
    #endregion

    // #region  List
    
    // public CommandStepFilterBuilder AddListSelect<T>(ListSelectionOptions<T> options, ViewModelCallbackCommandStepBaseOptions optionsBase)
    //     => Add(() => new ListSelectionCommandStep<T>(options, optionsBase));
    
    
    // public CommandStepFilterBuilder AddSingleSelect<T>(string userPrompt, Func<IServiceProvider, Task<IReadOnlyList<T>>> valueProvider, Func<T, string> displayName, Func<CommandStepCommitContext, T, Task> onCommit)
    //     => AddListSelect<T>(
    //         options: new() 
    //         { 
    //             ValueProvider = valueProvider,
    //             Mode = new ListSelectionMode.SingleSelect<T>() { OnCommit = onCommit },
    //             DisplayNameParser = displayName
    //         }, 
    //         optionsBase: new() { UserPrompt = userPrompt }
    //     );

    // public CommandStepFilterBuilder AddSingleSelect<T>(string userPrompt, IEnumerable<T> items, Func<T, string> displayName, Func<CommandStepCommitContext, T, Task> onCommit)
    //     => AddSingleSelect(
    //         valueProvider: async (sp) => items.ToList(),
    //         displayName: displayName,
    //         onCommit: onCommit,
    //         userPrompt: userPrompt
    //     );


    // public CommandStepFilterBuilder AddMultiSelect<T>(string userPrompt, Func<IServiceProvider, Task<IReadOnlyList<T>>> valueProvider, Func<T, string> displayName, Func<CommandStepCommitContext, IReadOnlyList<T>, Task> onCommit)
    //     => AddListSelect<T>(
    //         options: new() 
    //         { 
    //             ValueProvider = valueProvider,
    //             Mode = new ListSelectionMode.MultiSelect<T>() { OnCommit = onCommit },
    //             DisplayNameParser = displayName
    //         }, 
    //         optionsBase: new() { UserPrompt = userPrompt }
    //     );

    // public CommandStepFilterBuilder AddMultiSelect<T>(string userPrompt, IEnumerable<T> items, Func<T, string> displayName, Func<CommandStepCommitContext, IReadOnlyList<T>, Task> onCommit)
    //     => AddMultiSelect(
    //         valueProvider: async (sp) => items.ToList(),
    //         displayName: displayName,
    //         onCommit: onCommit,
    //         userPrompt: userPrompt
    //     );

    
    // #endregion

    #region File
    
    public CommandStepFilterBuilder AddFile(FileInputCommandStepOptions options)
        => Add(() => new FileInputCommandStep(options));
    
    public CommandStepFilterBuilder AddFile(string userPrompt, Func<CommandStepCommitContext, FileReference, Task> onUserCommit)
        => AddFile(new(){ UserPrompt = userPrompt, OnUserCommit = onUserCommit });

    #endregion


    public CommandStepRouter Build()
    {
        if(_stepDescriptors.Count == 0)
            throw new InvalidOperationException("Cannot build CommandStepRouter: no steps were configured.");

        Func<ICommandStep>[] stepFactories = new Func<ICommandStep>[_stepDescriptors.Count];

        for(int i = 0; i < _stepDescriptors.Count; i++)
        {
            var stepDescriptor = _stepDescriptors[i];
            var stepFactory = CompileStepFactory(stepDescriptor);

            stepFactories[i] = stepFactory;
        }

        return new CommandStepRouter(stepFactories);
    }

    private static Func<ICommandStep> CompileStepFactory(StepDescriptor descriptor)
    {
        if(descriptor.Filters.Count == 0)
            return descriptor.StepFactory;
        
        return () =>
        {
            ICommandStep step = descriptor.StepFactory();

            foreach(var filterFactory in descriptor.Filters)
            {
                var filter = filterFactory.Invoke();

                step = new FilterCommandStepDecorator(step, filter);
            }

            return step;
        };
    }


    public CommandStepRouterBuilder()
    {
        _stepDescriptors = [];
    }
}