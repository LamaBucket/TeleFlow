using TeleFlow.Commands.Statefull;
using TeleFlow.Commands.Statefull.StepCommands;
using TeleFlow.Factories;
using TeleFlow.Models.MultiStep;
using Telegram.Bot.Types;

namespace TeleFlow.Builders;

public class StepCommandFactoryBuilder
{
    private readonly List<Func<IFlowStep>> _stepCommandFactories;

    public StepCommandFactoryBuilder Add(Func<IFlowStep> factory, bool prepend = false)
    {
        _stepCommandFactories.Insert(prepend ? 0 : _stepCommandFactories.Count, factory);

        return this;
    }

    public StepCommandFactoryBuilder AddTextInput(TextInputStepOptions options) 
        => Add(() => new TextInputFlowStep(options));

    public StepCommandFactoryBuilder AddTextInput(string userPrompt, Func<FlowStepCommitContext, string, Task> onUserCommit) 
        => AddTextInput(new() { UserPrompt = userPrompt, OnUserCommit = onUserCommit });


    public StepCommandFactoryBuilder AddContactInput(ContactInputStepOptions options) 
        => Add(() => new ContactInputFlowStep(options));

    public StepCommandFactoryBuilder AddContactInput(string userPrompt, Func<FlowStepCommitContext, Contact, Task> onUserCommit) 
        => AddContactInput(new() { UserPrompt = userPrompt, OnUserCommit =  onUserCommit});


    public StepCommandFactory Build()
    {
        if(_stepCommandFactories.Count == 0)
            throw new Exception("No Step Commands Provided!");

        return new StepCommandFactory([.. _stepCommandFactories]);
    }

    public StepCommandFactoryBuilder()
    {
        _stepCommandFactories = [];
    }
}