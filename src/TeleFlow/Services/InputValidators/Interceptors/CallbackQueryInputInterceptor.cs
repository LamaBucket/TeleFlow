using TeleFlow.ViewModels.CallbackQuery;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Services.InputValidators;

public class CallbackQueryInputInterceptor<TButtonViewModel> : IUserInputValidator
{
    private readonly IUserInputValidator<TButtonViewModel> _vmInterceptor;


    public async Task<bool> ValidateUserInput(Update update)
    {
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null && update.CallbackQuery.Data is not null)
        {
            var vm = JsonConvert.DeserializeObject<TButtonViewModel>(update.CallbackQuery.Data);

            if (vm is null)
                return true;

            return await _vmInterceptor.ValidateUserInput(vm);
        }

        return true;
    }


    public CallbackQueryInputInterceptor(IUserInputValidator<TButtonViewModel> vmValidator)
    {
        _vmInterceptor = vmValidator;
    }
}