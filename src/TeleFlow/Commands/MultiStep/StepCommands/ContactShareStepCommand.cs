using TeleFlow.Exceptions;
using TeleFlow.Services.InputValidators;
using TeleFlow.Services.Markup;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Commands.MultiStep.StepCommands;

public class ContactShareStepCommand : StepCommandWithValidation
{
    private readonly IReplyMarkupManager _replyMarkupManager;


    private readonly Action<Contact> _onHandleUserMessage;

    private readonly string _onCommandCreatedMessage;

    private readonly string _shareContactButtonDisplayName;


    public override async Task OnCommandCreated()
    {
        await _replyMarkupManager.CreateReplyButtonMarkup(_onCommandCreatedMessage, new(KeyboardButton.WithRequestContact(_shareContactButtonDisplayName)));
    }

    protected override async Task HandleValidInput(Update args)
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

    public ContactShareStepCommand(IReplyMarkupManager replyMarkupManager,
                                   Action<Contact> onHandleUserMessage,
                                   string onCommandCreatedMessage,
                                   string shareContactButtonDisplayName,
                                   IUserInputValidator userInputValidator,
                                   StepCommand? next) : base(next, userInputValidator)
    {
        _replyMarkupManager = replyMarkupManager;
        _onHandleUserMessage = onHandleUserMessage;
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _shareContactButtonDisplayName = shareContactButtonDisplayName;
    }
}