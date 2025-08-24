using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories.CommandFactories;

public class StepCommandWithValidationFactory : ICommandFactory<StepCommand, Update, StepCommand>, IStateValidationDisplayNameProvider
{
    private readonly Func<string> _displayNameFunction;

    private readonly ICommandFactory<StepCommand, Update, StepCommand> _innerCommandFactory;


    public ICommandFactory<StepCommand, Update, StepCommand> StepToRestartCommandFactory => _innerCommandFactory;


    public StepCommand Create()
    {
        return _innerCommandFactory.Create();
    }

    public string GetDisplayName()
    {
        return _displayNameFunction();
    }

    public void SetContext(StepCommand args)
    {
        _innerCommandFactory.SetContext(args);
    }

    public StepCommandWithValidationFactory(ICommandFactory<StepCommand, Update, StepCommand> innerCommandFactory, Func<string> displayNameFunction)
    {
        _innerCommandFactory = innerCommandFactory;
        _displayNameFunction = displayNameFunction;
    }
}