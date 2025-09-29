
namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class LambdaInputValidator<T> : IUserInputValidator<T>
{
    private readonly Func<T, Task<bool>> _validator;

    public async Task<bool> ValidateUserInput(T update)
    {
        return await _validator.Invoke(update);
    }

    public LambdaInputValidator(Func<T, Task<bool>> validator)
    {
        _validator = validator;
    }
}