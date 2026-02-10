using TeleFlow.Commands.Flow.Steps.Options;
using TeleFlow.Commands.Flow.Steps.Resolvers;
using Telegram.Bot.Types;

namespace TeleFlow.Commands.Flow.Steps.Configuration;

public class FlowStepResolverBuilder
{
    private readonly List<Func<IFlowStep>> _stepCommandFactories;

    public FlowStepResolverBuilder Add(Func<IFlowStep> factory, bool prepend = false)
    {
        _stepCommandFactories.Insert(prepend ? 0 : _stepCommandFactories.Count, factory);

        return this;
    }

    public FlowStepResolverBuilder AddTextInput(TextInputStepOptions options) 
        => Add(() => new TextInputFlowStep(options));

    public FlowStepResolverBuilder AddTextInput(string userPrompt, Func<FlowStepCommitContext, string, Task> onUserCommit) 
        => AddTextInput(new() { UserPrompt = userPrompt, OnUserCommit = onUserCommit });


    public FlowStepResolverBuilder AddContactInput(ContactInputStepOptions options) 
        => Add(() => new ContactInputFlowStep(options));

    public FlowStepResolverBuilder AddContactInput(string userPrompt, Func<FlowStepCommitContext, Contact, Task> onUserCommit) 
        => AddContactInput(new() { UserPrompt = userPrompt, OnUserCommit =  onUserCommit});


    public FlowStepResolver Build()
    {
        if(_stepCommandFactories.Count == 0)
            throw new Exception("No Step Commands Provided!");

        return new FlowStepResolver([.. _stepCommandFactories]);
    }

    public FlowStepResolverBuilder()
    {
        _stepCommandFactories = [];
    }
}