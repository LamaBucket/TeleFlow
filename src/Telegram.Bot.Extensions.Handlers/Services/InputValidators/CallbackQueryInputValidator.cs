using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.InputValidators;

public class CallbackQueryInputValidator<TButtonViewModel> : IUserInputValidator
where TButtonViewModel : CallbackQueryViewModel
{
    private readonly string _wrongButtonClickMessage;

    private readonly IMessageService<string> _messageService;

    private readonly IUserInputValidator<TButtonViewModel>? _vmValidator;


    public async Task<bool> ValidateUserInput(Update update)
    {
        if (update.Type != UpdateType.CallbackQuery || update.CallbackQuery is null)
        {
            await _messageService.SendMessage("This command accepts only button clicks");
            return false;
        }

        var query = update.CallbackQuery;

        var queryData = query.Data ?? throw new NullReferenceException("Callback Query Data is null");

        var vm = JsonConvert.DeserializeObject<TButtonViewModel>(queryData);

        if (vm is null)
        {
            await _messageService.SendMessage(_wrongButtonClickMessage);
            return false;
        }

        if (_vmValidator is not null)
        {
            return await _vmValidator.ValidateUserInput(vm);
        }

        return true;
    }

    public CallbackQueryInputValidator(string wrongButtonClickMessage, IMessageService<string> messageService, IUserInputValidator<TButtonViewModel>? vmValidator = null)
    {
        _wrongButtonClickMessage = wrongButtonClickMessage;
        _messageService = messageService;
        _vmValidator = vmValidator;
    }
}
