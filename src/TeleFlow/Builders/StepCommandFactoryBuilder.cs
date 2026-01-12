using TeleFlow.Commands.MultiStep.StepCommands;
using TeleFlow.Commands.Statefull;
using TeleFlow.Factories;

namespace TeleFlow.Builders;

public class StepCommandFactoryBuilder
{
    private readonly List<Func<IStepCommand>> _stepCommandFactories;

    public StepCommandFactoryBuilder Add(Func<IStepCommand> factory, bool prepend = false)
    {
        _stepCommandFactories.Insert(prepend ? 0 : _stepCommandFactories.Count, factory);

        return this;
    }

    public StepCommandFactoryBuilder AddTextInput(string userPrompt, Func<IServiceProvider, string, Task> onUserInput) => Add(() => new TextInputStepCommand(userPrompt, onUserInput));

    public StepCommandFactory Build()
    {
        if(_stepCommandFactories.Count == 0)
            throw new Exception("No Step Commands Provided!");

        return new StepCommandFactory(_stepCommandFactories.ToArray());
    }

    public StepCommandFactoryBuilder()
    {
        _stepCommandFactories = [];
    }
}