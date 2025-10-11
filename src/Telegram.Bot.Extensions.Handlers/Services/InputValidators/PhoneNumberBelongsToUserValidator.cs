using Telegram.Bot.Extensions.Handlers.Exceptions;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class PhoneNumberBelongsToUserValidator : IUserInputValidator
{
    private readonly IMessageService<string> _messageService;

    private readonly IChatIdProvider _chatIdProvider;

    private readonly string _invalidInputMessage;

    private readonly string _phoneDoesNotBelongToUserMessage;

    public async Task<bool> ValidateUserInput(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message is null)
        {
            await _messageService.SendMessage(_invalidInputMessage);
            return false;
        }

        var message = update.Message;

        if (message.Type != MessageType.Contact || message.Contact is null)
        {
            await _messageService.SendMessage(_invalidInputMessage);
            return false;
        }

        bool validationResult = message.Contact.UserId == _chatIdProvider.GetChatId();

        if (!validationResult)
            await _messageService.SendMessage(_phoneDoesNotBelongToUserMessage);

        return validationResult;
    }


    public PhoneNumberBelongsToUserValidator(IMessageService<string> messageService, IChatIdProvider chatIdProvider, string invalidInputMessage, string phoneDoesNotBelongToUserMessage)
    {
        _messageService = messageService;
        _chatIdProvider = chatIdProvider;
        _invalidInputMessage = invalidInputMessage;
        _phoneDoesNotBelongToUserMessage = phoneDoesNotBelongToUserMessage;
    }
}