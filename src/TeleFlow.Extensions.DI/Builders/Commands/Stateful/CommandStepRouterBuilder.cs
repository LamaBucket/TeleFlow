using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.StepRegistration;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful;

public class CommandStepRouterBuilder
{
    private readonly List<IStepRegistration> _stepDescriptors;


    public StepFilterBuilder Add(Func<ICommandStep> factory, bool prepend = false)
    {
        PlainStepRegistration registration = new(factory);

        _stepDescriptors.Insert(prepend ? 0 : _stepDescriptors.Count, registration);

        return new StepFilterBuilder(registration, this);
    }

    public StepWithViewModelFilterBuilder<TViewModel> Add<TViewModel>(Func<IStepRenderService<TViewModel>> renderServiceFactory, Func<IStepRenderService<TViewModel>, StepBase<TViewModel>> factory, bool prepend = false) 
        where TViewModel : StepViewModel
    {
        StepWithRenderRegistration<TViewModel> registration = new(factory, renderServiceFactory);
        
        _stepDescriptors.Insert(prepend ? 0 : _stepDescriptors.Count, registration);

        return new StepWithViewModelFilterBuilder<TViewModel>(registration);
    }

    #region  Text
    
    // public CommandStepFilterBuilder AddTextInput(TextInputCommandStepOptions options) 
    //     => Add(() => new TextInputCommandStep(options));

    // public CommandStepFilterBuilder AddTextInput(string userPrompt, Func<CommandStepCommitContext, string, Task> onUserCommit) 
    //     => AddTextInput(new() { UserPrompt = userPrompt, OnUserCommit = onUserCommit });
    
    #endregion

    #region  Contact
    
    // public CommandStepFilterBuilder AddContactInput(ContactInputStepOptions options) 
    //     => Add(() => new ContactInputStep(options));

    // public CommandStepFilterBuilder AddContactInput(string userPrompt, Func<CommandStepCommitContext, Contact, Task> onUserCommit) 
    //     => AddContactInput(new() { UserPrompt = userPrompt, OnUserCommit =  onUserCommit});
    
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
    
    // public CommandStepFilterBuilder AddFile(FileInputCommandStepOptions options)
    //     => Add(() => new FileInputCommandStep(options));
    
    // public CommandStepFilterBuilder AddFile(string userPrompt, Func<CommandStepCommitContext, FileReference, Task> onUserCommit)
    //     => AddFile(new(){ UserPrompt = userPrompt, OnUserCommit = onUserCommit });

    #endregion


    public CommandStepRouter Build()
    {
        if(_stepDescriptors.Count == 0)
            throw new InvalidOperationException("Cannot build CommandStepRouter: no steps were configured.");

        Func<ICommandStep>[] stepFactories = new Func<ICommandStep>[_stepDescriptors.Count];

        for(int i = 0; i < _stepDescriptors.Count; i++)
        {
            var stepDescriptor = _stepDescriptors[i];
            var stepFactory = stepDescriptor.CompileFactory();

            stepFactories[i] = stepFactory;
        }

        return new CommandStepRouter(stepFactories);
    }

    public CommandStepRouterBuilder()
    {
        _stepDescriptors = [];
    }
}