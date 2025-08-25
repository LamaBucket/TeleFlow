using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram.Commands.MultiStep.StepCommands;

public class TextInputStepCommand : StepCommand
{
    private readonly IMessageService<string> _messageService;

    private readonly string _onCommandCreatedMessage;

    private readonly Func<string, Task> _onHandleUserMessage;

    public override async Task OnCommandCreated()
    {
        await _messageService.SendMessage(_onCommandCreatedMessage);
    }

    protected override async Task HandleCurrentStep(Update args)
    {
        if(args.Type != UpdateType.Message || args.Message is null)
            throw new Exception("Awaits messages only");

        await _onHandleUserMessage(args.Message.Text ?? throw new Exception("Empty Message"));

        await FinalizeCommand();
    }


    public TextInputStepCommand(IMessageService<string> messageService,
                                string onCommandCreatedMessage,
                                Func<string, Task> onHandleUserMessage,
                                StepCommand? next) : base(next)
    {
        _messageService = messageService;
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _onHandleUserMessage = onHandleUserMessage;
    }
}