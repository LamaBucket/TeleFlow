using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput;

public class ContactInputCommandStep : ICommandStep
{
    private readonly ContactInputCommandStepOptions _options;

    public async Task<CommandStepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        Contact? contact = update.Message.Contact;

        if(contact is null)
        {
            if(_options.TryParseContactFromText is null)
                return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoContactProvidedMessage);
            
            if(update.Message.Text is null)
                return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);
            
            contact = _options.TryParseContactFromText.Invoke(update.Message.Text);

            if(contact is null)
                return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.InvalidTextContactMessage);
        }

        CommandStepCommitContext context = new(args.ServiceProvider);
        await _options.OnUserCommit.Invoke(context, contact);

        await RemoveReplyMarkupButton(args.ServiceProvider);
        
        return GetSuccessStepResult();
    }

    private static async Task RemoveReplyMarkupButton(IServiceProvider sp)
    {
        var msgSendService = sp.GetRequiredService<IMessageSender>();
        var msgEditService = sp.GetRequiredService<IMessageEditor>();

        OutgoingMessage payload = new()
        { 
            Text = "Hold tight, we are removing the inline button...", 
            ReplyMarkup = ReplyKeyboardBuilder.Remove() 
        };

        Message msg = await msgSendService.SendMessage(payload);        
        await msgEditService.Delete(msg.MessageId);
    }

    public virtual CommandStepResult GetSuccessStepResult()
    {
        return CommandStepResult.Next;
    }


    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var messageService = serviceProvider.GetRequiredService<IMessageSender>();

        OutgoingMessage msg = new()
        {
            Text = _options.UserPrompt,
            ReplyMarkup = BuildReplyMarkup()
        };

        await messageService.SendMessage(msg);
    }

    private ReplyKeyboardMarkup? BuildReplyMarkup()
    {
        if(_options.ShareContactButtonText is null)
            return null;

        ReplyKeyboardBuilder builder = new();
        builder.ContactRequestButton(_options.ShareContactButtonText);

        return builder.Build();
    }


    public ContactInputCommandStep(ContactInputCommandStepOptions options)
    {
        _options = options;
    }
}