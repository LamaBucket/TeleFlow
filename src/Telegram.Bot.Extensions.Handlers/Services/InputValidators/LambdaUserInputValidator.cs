
namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class LambdaUserInputValidator<TUpdate> : IUserInputValidator<TUpdate>
{
    private readonly Func<TUpdate, Task<bool>> _lambdaValidator;

    public async Task<bool> ValidateUserInput(TUpdate update)
    {
        return await _lambdaValidator.Invoke(update);
    }

    public LambdaUserInputValidator(Func<TUpdate, Task<bool>> lambdaValidator)
    {
        _lambdaValidator = lambdaValidator;
    }
}
