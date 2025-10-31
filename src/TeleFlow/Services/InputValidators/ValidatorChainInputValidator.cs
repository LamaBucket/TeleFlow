using Telegram.Bot.Types;

namespace TeleFlow.Services.InputValidators;

public class ValidatorChainInputValidator : IUserInputValidator
{
    private readonly IUserInputValidator[] _validators;

    public async Task<bool> ValidateUserInput(Update update)
    {
        foreach (var validator in _validators)
        {
            bool isValid = await validator.ValidateUserInput(update);

            if (!isValid)
                return false;
        }
        return true;
    }

    public ValidatorChainInputValidator(params IUserInputValidator[] validators)
    {
        _validators = validators;
    }
}