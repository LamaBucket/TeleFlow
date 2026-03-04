using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public class TextInputCommandStep : StepBase<TextInputCommandStepViewModel>
{
    private readonly TextInputCommandStepOptions _options;

    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<TextInputCommandStepViewModel> state)
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

        return GetSuccessStepResult();;
    }

    private async Task SetStateInputTextAndRerender(IServiceProvider sp, StepState<TextInputCommandStepViewModel> state, string value)
    {
        state = state with
        {
            ViewModel = state.ViewModel with
            {
                TextEntered = value
            }
        };
    
        await UpsertAndRerender(sp, state);
    }

    protected virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;


    protected override Task<TextInputCommandStepViewModel> CreateDefaultViewModel(IServiceProvider sp)
        => Task.FromResult(TextInputCommandStepViewModel.Default);


    public TextInputCommandStep(TextInputCommandStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}