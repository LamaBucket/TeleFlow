using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Messaging;
using TeleFlow.Commands.Flow.Steps.Options;
using TeleFlow.Commands.Statefull;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Commands.Statefull.StepCommands;

public class TextInputFlowStep : IFlowStep
{
    private readonly TextInputStepOptions _options;

    public async Task<FlowStepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        if(update.Message.Text is null)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);

        await _options.OnUserCommit.Invoke(new(args.ServiceProvider), update.Message.Text);

        return GetSuccessStepResult();
    }

    public virtual FlowStepResult GetSuccessStepResult()
    {
        return FlowStepResult.Next;
    }


    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var messageService = serviceProvider.GetRequiredService<IMessageSender>();

        await messageService.SendMessage(_options.UserPrompt);
    }


    public TextInputFlowStep(TextInputStepOptions options)
    {
        _options = options;
    }
}