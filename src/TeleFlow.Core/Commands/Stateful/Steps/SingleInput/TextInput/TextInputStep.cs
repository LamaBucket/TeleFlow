using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.TextInput;

public class TextInputStep : SingleInputStep<string>
{
    private readonly TextInputStepOptions _options;

    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<SingleInputStepData<string>> state)
    {
        var update = context.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        if(update.Message.Text is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);

        var userInput = update.Message.Text;

        await SetInputAndRerender(context.ServiceProvider, state, userInput);
        await FinalizeStep(context.ServiceProvider);

        await _options.OnUserCommit.Invoke(new(context.ServiceProvider), update.Message.Text);

        return GetSuccessStepResult();
    }

    protected virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;


    protected override Task<SingleInputStepData<string>> CreateDefaultStepData(IServiceProvider sp)
        => Task.FromResult(SingleInputStepData<string>.CreateDefault());


    public TextInputStep(TextInputStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}