using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Commands.MultiStep.StepCommands;

public class ContactShareStepCommand : StepCommand
{
    private readonly IMessageService<Tuple<string, KeyboardButton>> _messageService;

    private readonly IReplyMarkupManager _replyMarkupManager;


    private readonly Action<Contact> _onHandleUserMessage;

    private readonly string _onCommandCreatedMessage;

    private readonly string _shareContactButtonDisplayName;


    public override async Task OnCommandCreated()
    {
        await _messageService.SendMessage(new(_onCommandCreatedMessage, KeyboardButton.WithRequestContact(_shareContactButtonDisplayName)));
    }

    protected override async Task HandleCurrentStep(Update args)
    {
        if(args.Type != UpdateType.Message || args.Message is null)
            throw new Exception("Awaits messages only");

        var message = args.Message;
        
        if(message.Type != MessageType.Contact || message.Contact is null)
            throw new Exception("Awaits contacts only");


        await _replyMarkupManager.ClearReplyButtons();       

        var contact = message.Contact;

        _onHandleUserMessage(contact);

        await FinalizeCommand();
    }

    public ContactShareStepCommand(IMessageService<Tuple<string, KeyboardButton>> messageService,
                                   IReplyMarkupManager replyMarkupManager,
                                   Action<Contact> onHandleUserMessage,
                                   string onCommandCreatedMessage,
                                   string shareContactButtonDisplayName)
    {
        _messageService = messageService;
        _replyMarkupManager = replyMarkupManager;
        _onHandleUserMessage = onHandleUserMessage;
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _shareContactButtonDisplayName = shareContactButtonDisplayName;
    }
}