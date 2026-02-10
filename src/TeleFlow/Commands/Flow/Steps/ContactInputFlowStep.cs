using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Messaging;
using TeleFlow.Commands.Flow.Steps.Options;
using TeleFlow.Pipeline.Contexts;
using TeleFlow.Presentation.Builders;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Commands.Flow.Steps;

public class ContactInputFlowStep : IFlowStep
{
    private readonly ContactInputStepOptions _options;

    public async Task<FlowStepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);
        
        Contact? contact = update.Message.Contact;

        if(contact is null)
        {
            if(_options.TryParseContactFromText is null)
                return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.NoContactProvidedMessage);
            
            if(update.Message.Text is null)
                return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.NoTextProvidedMessage);
            
            contact = _options.TryParseContactFromText.Invoke(update.Message.Text);

            if(contact is null)
                return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.InvalidTextContactMessage);
        }

        FlowStepCommitContext context = new(args.ServiceProvider);
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

    public virtual FlowStepResult GetSuccessStepResult()
    {
        return FlowStepResult.Next;
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


    public ContactInputFlowStep(ContactInputStepOptions options)
    {
        _options = options;
    }
}