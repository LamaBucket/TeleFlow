using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Builders.Keyboards;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.Messaging;
using TeleFlow.Models.MultiStep;
using TeleFlow.Services;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Commands.Statefull.StepCommands;

public class ContactInputStep : IStepCommand
{
    private readonly ContactInputStepOptions _options;

    public async Task<StepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        Contact? contact = update.Message.Contact;

        if(contact is null)
        {
            if(_options.TryParseContactFromText is null)
                return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.NoContactProvidedMessage);
            
            if(update.Message.Text is null)
                return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);
            
            contact = _options.TryParseContactFromText.Invoke(update.Message.Text);

            if(contact is null)
                return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.InvalidTextContactMessage);
        }

        StepCommitContext context = new(args.ServiceProvider);
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

    public virtual StepResult GetSuccessStepResult()
    {
        return StepResult.Next;
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


    public ContactInputStep(ContactInputStepOptions options)
    {
        _options = options;
    }
}

public class ContactInputStepOptions
{
    public required string UserPrompt { get; init; }

    public required Func<StepCommitContext, Contact, Task> OnUserCommit { get; init; }


    public string? ShareContactButtonText { get; init; } = null;

    public Func<string, Contact?>? TryParseContactFromText { get; init; } = null;


    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";
    public string NoContactProvidedMessage { get; init; } = "Please share a contact.";
    public string NoTextProvidedMessage { get; init; } = "Please share a contact or provide contact text";
    public string InvalidTextContactMessage { get; init; } = "Please share a contact or provide a contact text in a valid format.";
}