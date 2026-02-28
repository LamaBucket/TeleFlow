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
        var msgSendService = sp.GetRequiredService<IMessageSendService>();
        var msgDeleteService = sp.GetRequiredService<IMessageDeleteService>();

        var replyKeyboardRemoveMessage = new ReplyMarkupMessage()
        { 
            Text = "Hold tight, we are removing the inline button...", 
            Markup = new ReplyMarkupSpec.Remove(new()) 
        };

        Message msg = await msgSendService.SendMessage(replyKeyboardRemoveMessage);        
        await msgDeleteService.Delete(msg.MessageId);
    }

    public virtual CommandStepResult GetSuccessStepResult()
    {
        return CommandStepResult.Next;
    }


    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var messageService = serviceProvider.GetRequiredService<IMessageSendService>();

        if(_options.ShareContactButtonText is not null)
        {
            ReplyMarkupMessage msg = new()
            {
                Text = _options.UserPrompt,
                Markup = BuildReplyMarkup()
            };   
            
            await messageService.SendMessage(msg);
        }
        else
        {
            InlineMarkupMessage msg = new(){ Text = _options.UserPrompt };
            await messageService.SendMessage(msg);   
        }
    }

    private ReplyMarkupSpec BuildReplyMarkup()
    {
        if(_options.ShareContactButtonText is null)
            throw new Exception();

        ReplyKeyboardBuilder builder = new();
        builder.ContactRequestButton(_options.ShareContactButtonText);

        return new ReplyMarkupSpec.Keyboard(builder.Build());
    }


    public ContactInputCommandStep(ContactInputCommandStepOptions options)
    {
        _options = options;
    }
}