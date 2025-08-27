using LisBot.Common.Telegram.Exceptions;
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
            throw new InvalidUserInput("This command accepts only messages.");

        var message = args.Message;
        

        var contact = ParseUserMessage(message);        

        await _replyMarkupManager.ClearReplyButtons();       

        _onHandleUserMessage(contact);

        await FinalizeCommand();
    }

    protected virtual Contact ParseUserMessage(Message message)
    {
        if (message.Type != MessageType.Contact || message.Contact is null)
            throw new InvalidUserInput("This command accepts only contacts.");
            
        return message.Contact;
    }

    public ContactShareStepCommand(IMessageService<Tuple<string, KeyboardButton>> messageService,
                                   IReplyMarkupManager replyMarkupManager,
                                   Action<Contact> onHandleUserMessage,
                                   string onCommandCreatedMessage,
                                   string shareContactButtonDisplayName,
                                   StepCommand? next) : base(next)
    {
        _messageService = messageService;
        _replyMarkupManager = replyMarkupManager;
        _onHandleUserMessage = onHandleUserMessage;
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _shareContactButtonDisplayName = shareContactButtonDisplayName;
    }
}