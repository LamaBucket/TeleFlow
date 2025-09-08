using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class CallbackQueryInputInterceptor<TButtonViewModel> : IUserInputValidator
{
    private readonly IUserInputValidator<TButtonViewModel> _vmValidator;

    public async Task<bool> ValidateUserInput(Update update)
    {
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null)
        {
            var query = update.CallbackQuery;

            var queryData = query.Data ?? throw new NullReferenceException("Callback Query Data is null");

            var vm = JsonConvert.DeserializeObject<TButtonViewModel>(queryData);

            if (vm is not null)
            {
                return await _vmValidator.ValidateUserInput(vm);
            }
        }

        return true;
    }

    public CallbackQueryInputInterceptor(IUserInputValidator<TButtonViewModel> vmValidator)
    {
        _vmValidator = vmValidator;
    }
}