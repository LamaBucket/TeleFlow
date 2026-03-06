using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public class TextInputStep : StatefulStep<TextInputStepData>
{
    private readonly TextInputStepOptions _options;

    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<TextInputStepData> state)
    {
        var update = context.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        if(update.Message.Text is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);

        var userInput = update.Message.Text;

        await SetStateInputTextAndRerender(context.ServiceProvider, state, userInput);
        await FinalizeStep(context.ServiceProvider);

        await _options.OnUserCommit.Invoke(new(context.ServiceProvider), update.Message.Text);

        return GetSuccessStepResult();
    }

    private async Task SetStateInputTextAndRerender(IServiceProvider sp, StepState<TextInputStepData> state, string value)
    {
        state = state with
        {
            StepData = state.StepData with
            {
                TextEntered = value
            }
        };
    
        await UpsertAndRerender(sp, state);
    }

    protected virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;


    protected override Task<TextInputStepData> CreateDefaultStepData(IServiceProvider sp)
        => Task.FromResult(TextInputStepData.Default);


    public TextInputStep(TextInputStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}