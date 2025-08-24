using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep.StepCommands;

public class StepCommandWithDisplayName : StepCommand, IStateValidationDisplayNameProvider
{
    private readonly Func<string> _displayNameProvider;

    private readonly StepCommand _innerStepCommand;

    private readonly ICommandFactory<StepCommand, Update, StepCommand> _innerStepCommandFactory;


    public ICommandFactory<StepCommand, Update, StepCommand> StepToRestartCommandFactory => _innerStepCommandFactory;


    public string GetDisplayName()
    {
        return _displayNameProvider();
    }

    public override async Task OnCommandCreated()
    {
        await _innerStepCommand.OnCommandCreated();
    }

    protected override async Task HandleCurrentStep(Update args)
    {
        await _innerStepCommand.Handle(args);
    }

    private async Task OnInnerCommandFinished()
    {
        _innerStepCommand.CommandFinished -= OnInnerCommandFinished;
        await FinalizeCommand();
    }

    public StepCommandWithDisplayName(ICommandFactory<StepCommand, Update, StepCommand> innerStepCommandFactory, Func<string> displayNameProvider)
    {
        _innerStepCommandFactory = innerStepCommandFactory;
        _displayNameProvider = displayNameProvider;

        _innerStepCommand = StepToRestartCommandFactory.Create();
        _innerStepCommand.CommandFinished += OnInnerCommandFinished;
    }
}
