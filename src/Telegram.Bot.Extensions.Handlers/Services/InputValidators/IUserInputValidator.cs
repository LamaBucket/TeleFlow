using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public interface IUserInputValidator
{
    Task<bool> ValidateUserInput(Update update);
}