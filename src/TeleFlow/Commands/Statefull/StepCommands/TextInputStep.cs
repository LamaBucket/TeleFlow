using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Commands.Statefull;
using TeleFlow.Exceptions;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.MultiStep;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Commands.Statefull.StepCommands;

public class TextInputStep : IStepCommand
{
    private readonly string _userPrompt;

    private readonly Func<IServiceProvider, string, Task> _onUserInput;

    public async Task<StepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if(update.Type != UpdateType.Message || update.Message is null || update.Message.Text is null)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, "Please provide a valid text input.");

        await _onUserInput(args.ServiceProvider, update.Message.Text);

        return GetSuccessStepResult();
    }

    public virtual StepResult GetSuccessStepResult()
    {
        return StepResult.Next;
    }


    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var messageService = serviceProvider.GetService<IMessageService<string>>() ?? throw new Exception("Unable to get MessageService");

        await messageService.SendMessage(_userPrompt);
    }


    public TextInputStep(string userPrompt,
                                Func<IServiceProvider, string, Task> onUserInput)
    {
        _userPrompt = userPrompt;
        _onUserInput = onUserInput;
    }
}