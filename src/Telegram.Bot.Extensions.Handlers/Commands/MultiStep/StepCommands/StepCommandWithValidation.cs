using Telegram.Bot.Extensions.Handlers.Services.InputValidators;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Commands.MultiStep.StepCommands;

public abstract class StepCommandWithValidation : StepCommand
{
    private readonly IUserInputValidator _userInputValidator;


    protected override async Task HandleCurrentStep(Update args)
    {
        if (await _userInputValidator.ValidateUserInput(args))
        {
            await HandleValidInput(args);
        }
    }

    protected abstract Task HandleValidInput(Update args);
    

    protected StepCommandWithValidation(StepCommand? next, IUserInputValidator? userInputValidator = null) : base(next)
    {
        _userInputValidator = userInputValidator ?? new NoValidationUserValidator();
    }
}