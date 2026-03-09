using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.ContactInput;

public class ContactInputStep : SingleInputStep<ContactInputStepData, Contact>
{
    private readonly ContactInputStepOptions _options;

    private bool IsContactShareButtonUsed => !string.IsNullOrWhiteSpace(_options.ShareContactButtonText);

    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<ContactInputStepData> state)
    {
        var update = context.Update;

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

        if (IsContactShareButtonUsed)
        {
            await RemoveShareContactReplyButton(context.ServiceProvider, state.StepData.ShareContactReplyButtonMessageId);
            state = state with { StepData = state.StepData with { ShareContactReplyButtonMessageId = null } };   
        }
        

        await SetInputAndRerender(context.ServiceProvider, state, contact);
        await FinalizeStep(context.ServiceProvider);
        

        await _options.OnUserCommit.Invoke(new(context.ServiceProvider), contact);
        
        return GetSuccessStepResult();
    }

    private static async Task RemoveShareContactReplyButton(IServiceProvider sp, int? messageId)
    {
        var msgDeleteService = sp.GetRequiredService<IMessageDeleteService>();

        if (messageId.HasValue)
        {
            await msgDeleteService.Delete(messageId.Value);
        }
        else
        {
            var msgSendService = sp.GetRequiredService<IMessageSendService>();
            
            var replyKeyboardRemoveMessage = new ReplyMarkupMessage()
            { 
                Text = "Use the button below to share your number", 
                Markup = new ReplyMarkupSpec.Remove(new()) 
            };
            
            Message msg = await msgSendService.SendMessage(replyKeyboardRemoveMessage);  
            await msgDeleteService.Delete(msg.MessageId); 
        }
    }

    protected virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;
    

    protected override async Task<ContactInputStepData> CreateDefaultStepData(IServiceProvider sp)
    {
        int? msgId = null;

        if(IsContactShareButtonUsed)    
            msgId = await ShowShareContactReplyButton(sp);

        return new(null, msgId);
    }

    private async Task<int?> ShowShareContactReplyButton(IServiceProvider sp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.ShareContactButtonText, nameof(_options.ShareContactButtonText));

        var msgSendService = sp.GetRequiredService<IMessageSendService>();

        ReplyKeyboardBuilder builder = new();
        builder.ContactRequestButton(_options.ShareContactButtonText);

        var replyKeyboardRemoveMessage = new ReplyMarkupMessage()
        { 
            Text = "Use the button below to share your number", 
            Markup = new ReplyMarkupSpec.Keyboard(builder.Build()) 
        };

        Message msg = await msgSendService.SendMessage(replyKeyboardRemoveMessage);        
        return msg.MessageId;
    }


    public ContactInputStep(ContactInputStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}