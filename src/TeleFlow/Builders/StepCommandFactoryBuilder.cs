using TeleFlow.Commands.Statefull;
using TeleFlow.Commands.Statefull.StepCommands;
using TeleFlow.Factories;
using TeleFlow.Models.MultiStep;

namespace TeleFlow.Builders;

public class StepCommandFactoryBuilder
{
    private readonly List<Func<IStepCommand>> _stepCommandFactories;

    public StepCommandFactoryBuilder Add(Func<IStepCommand> factory, bool prepend = false)
    {
        _stepCommandFactories.Insert(prepend ? 0 : _stepCommandFactories.Count, factory);

        return this;
    }

    public StepCommandFactoryBuilder AddTextInput(string userPrompt, Func<StepCommitContext, string, Task> onUserInput) 
        => Add(() => new TextInputStep(userPrompt, onUserInput));

    public StepCommandFactoryBuilder AddContactInput(ContactInputStepOptions options) 
        => Add(() => new ContactInputStep(options));

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