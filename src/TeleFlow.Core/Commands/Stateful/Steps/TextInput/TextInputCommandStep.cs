using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput;

public class TextInputCommandStep : ICommandStep
{
    private readonly TextInputCommandStepOptions _options;

    public async Task<CommandStepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        if(update.Message.Text is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);

        await _options.OnUserCommit.Invoke(new(args.ServiceProvider), update.Message.Text);

        return GetSuccessStepResult();
    }

    public virtual CommandStepResult GetSuccessStepResult()
    {
        return CommandStepResult.Next;
    }


    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var messageService = serviceProvider.GetRequiredService<IMessageSender>();

        await messageService.SendMessage(_options.UserPrompt);
    }


    public TextInputCommandStep(TextInputCommandStepOptions options)
    {
        _options = options;
    }
}