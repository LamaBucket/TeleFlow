using Telegram.Bot.Types;

namespace TeleFlow.Services.InputValidators;

public class NoValidationUserValidator : IUserInputValidator
{
    public Task<bool> ValidateUserInput(Update update)
    {
        return Task.FromResult(true);
    }
}