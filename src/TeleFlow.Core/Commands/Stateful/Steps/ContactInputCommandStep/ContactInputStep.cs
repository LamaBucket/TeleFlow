using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInputCommandStep;

public class ContactInputStep : StepBase<ContactInputStepViewModel>
{
    private readonly ContactInputStepOptions _options;

    private bool IsContactShareButtonUsed => !string.IsNullOrWhiteSpace(_options.ShareContactButtonText);


    private ReplyMarkupSpec BuildReplyMarkup()
    {
        if(_options.ShareContactButtonText is null)
            throw new Exception();

        ReplyKeyboardBuilder builder = new();
        builder.ContactRequestButton(_options.ShareContactButtonText);

        return new ReplyMarkupSpec.Keyboard(builder.Build());
    }

    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<ContactInputStepViewModel> state)
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
        
        if(IsContactShareButtonUsed)
            await RemoveShareContactReplyButton(context.ServiceProvider);
        

        await SetStateSharedContactAndRerender(context.ServiceProvider, state, contact);
        await FinalizeStep(context.ServiceProvider);
        

        await _options.OnUserCommit.Invoke(new(context.ServiceProvider), contact);
        
        return GetSuccessStepResult();
    }

    private static async Task RemoveShareContactReplyButton(IServiceProvider sp)
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

    private async Task SetStateSharedContactAndRerender(IServiceProvider sp, StepState<ContactInputStepViewModel> state, Contact value)
    {
        state.ViewModel.ContactShared = value;
        await UpsertAndRerender(sp, state);
    }

    public virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;


    public override async Task OnEnter(IServiceProvider serviceProvider)
    {
        await base.OnEnter(serviceProvider);

        if(IsContactShareButtonUsed)
            await ShowShareContactReplyButton(serviceProvider);
    }

    private async Task ShowShareContactReplyButton(IServiceProvider sp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.ShareContactButtonText, nameof(_options.ShareContactButtonText));

        var msgSendService = sp.GetRequiredService<IMessageSendService>();
        var msgDeleteService = sp.GetRequiredService<IMessageDeleteService>();

        ReplyKeyboardBuilder builder = new();
        builder.ContactRequestButton(_options.ShareContactButtonText);

        var replyKeyboardRemoveMessage = new ReplyMarkupMessage()
        { 
            Text = "Hold tight, we are adding the inline button...", 
            Markup = new ReplyMarkupSpec.Keyboard(builder.Build()) 
        };

        Message msg = await msgSendService.SendMessage(replyKeyboardRemoveMessage);        
        await msgDeleteService.Delete(msg.MessageId);
    }


    protected override Task<ContactInputStepViewModel> CreateDefaultViewModel(IServiceProvider sp)
        => Task.FromResult<ContactInputStepViewModel>(new());


    public ContactInputStep(ContactInputStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}