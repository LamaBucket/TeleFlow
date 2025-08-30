using LisBot.Common.Telegram.Exceptions;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class PhoneNumberBelongsToUserValidator : IUserInputValidator
{
    private readonly IMessageService<string> _messageService;

    private readonly IChatIdProvider _chatIdProvider;

    public async Task<bool> ValidateUserInput(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message is null)
            throw new InvalidUserInput("This command accepts only messages.");

        var message = update.Message;

        if (message.Type != MessageType.Contact || message.Contact is null)
            throw new InvalidUserInput("This command accepts only contacts.");

        bool validationResult = message.Contact.UserId == _chatIdProvider.GetChatId();

        if (!validationResult)
            await _messageService.SendMessage("This phone number does not belong to you.");
    
        return validationResult;
    }


    public PhoneNumberBelongsToUserValidator(IMessageService<string> messageService, IChatIdProvider chatIdProvider)
    {
        _messageService = messageService;
        _chatIdProvider = chatIdProvider;
    }
}