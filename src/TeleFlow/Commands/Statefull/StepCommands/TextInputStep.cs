using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Commands.Statefull;
using TeleFlow.Exceptions;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Commands.Statefull.StepCommands;

public class TextInputStep : IStepCommand
{
    private readonly TextInputStepOptions _options;

    public async Task<StepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        if(update.Message.Text is null)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);

        await _options.OnUserCommit.Invoke(new(args.ServiceProvider), update.Message.Text);

        return GetSuccessStepResult();
    }

    public virtual StepResult GetSuccessStepResult()
    {
        return StepResult.Next;
    }


    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var messageService = serviceProvider.GetRequiredService<IMessageSender>();

        await messageService.SendMessage(_options.UserPrompt);
    }


    public TextInputStep(TextInputStepOptions options)
    {
        _options = options;
    }
}

public class TextInputStepOptions
{
    public required string UserPrompt { get; init; }

    public required Func<StepCommitContext, string, Task> OnUserCommit { get; set; }

    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";

    public string NoTextProvidedMessage { get; init; } = "This command accepts only text";
}