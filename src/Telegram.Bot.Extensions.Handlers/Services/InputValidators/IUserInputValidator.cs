using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public interface IUserInputValidator<TUpdate>
{
    Task<bool> ValidateUserInput(TUpdate update);
}

public interface IUserInputValidator : IUserInputValidator<Update>
{
    
}