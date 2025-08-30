using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class NoValidationUserValidator : IUserInputValidator
{
    public Task<bool> ValidateUserInput(Update update)
    {
        return Task.FromResult(true);
    }
}