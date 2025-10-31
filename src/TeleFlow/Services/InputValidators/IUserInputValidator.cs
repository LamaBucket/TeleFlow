using Telegram.Bot.Types;

namespace TeleFlow.Services.InputValidators;

public interface IUserInputValidator<TUpdate>
{
    Task<bool> ValidateUserInput(TUpdate update);
}

public interface IUserInputValidator : IUserInputValidator<Update>
{
    
}