using System.Text.RegularExpressions;
using Telegram.Bot.Extensions.Handlers.Services.InputValidators;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class RegexTextMessageUserValidator : IUserInputValidator
{
    public static RegexTextMessageUserValidator GetRegexWithoutNumbers(string regexNotMatchMessage, IMessageService<string> messageService)
    {
        return new RegexTextMessageUserValidator(@"^[^\d]*$", regexNotMatchMessage, messageService);
    }

    public static RegexTextMessageUserValidator GetRegexForNumbersOnly(string regexNotMatchMessage, IMessageService<string> messageService)
    {
        return new RegexTextMessageUserValidator(@"^\d*$", regexNotMatchMessage, messageService);
    }

    public static RegexTextMessageUserValidator GetRegexForDecimalNumbers(string regexNotMatchMessage, IMessageService<string> messageService)
    {
        return new RegexTextMessageUserValidator(@"^\d+(\.\d+)?$", regexNotMatchMessage, messageService);
    }

    public static RegexTextMessageUserValidator GetRegexForAnyTextValidator(string notTextMessage, IMessageService<string> messageService)
    {
        return new RegexTextMessageUserValidator(@"*", notTextMessage, messageService);
    }


    private readonly string _pattern;

    private readonly string _regexNotMatchMessage;

    private readonly IMessageService<string> _messageService;


    public async Task<bool> ValidateUserInput(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message is null)
        {
            await _messageService.SendMessage("This command accepts only text messages.");
            return false;
        }

        var message = update.Message;

        if (message.Type != MessageType.Text || message.Text is null)
        {
            await _messageService.SendMessage("This command accepts only text messages.");
            return false;
        }

        bool isValid = Regex.IsMatch(message.Text, _pattern);

        if (!isValid)
        {
            await _messageService.SendMessage(_regexNotMatchMessage);
        }

        return isValid;
    }

    public RegexTextMessageUserValidator(string pattern, string regexNotMatchMessage, IMessageService<string> messageService)
    {
        _pattern = pattern;
        _regexNotMatchMessage = regexNotMatchMessage;
        _messageService = messageService;
    }
}
