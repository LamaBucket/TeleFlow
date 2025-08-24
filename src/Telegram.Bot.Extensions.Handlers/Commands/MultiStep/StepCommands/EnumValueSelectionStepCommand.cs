using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep.StepCommands;

public class EnumValueSelectionStepCommand<TEnum> : StepCommand 
    where TEnum : Enum
{
    private readonly IMessageService<Message> _messageService;

    private readonly EnumValueSelectionMessageFormatter<TEnum> _formatter;

    private readonly string? _noSelectButtonName;


    private readonly Func<TEnum, Task>? _onHandleUserSelectCannotBeNull;

    private readonly Func<Tuple<TEnum>?, Task>? _onHandleUserSelect;


    public override async Task OnCommandCreated()
    {
        await _messageService.SendMessage(_formatter.GenerateMessage());
    }

    protected override async Task HandleCurrentStep(Update args)
    {
        if(_noSelectButtonName is null)
        {
            if(_onHandleUserSelectCannotBeNull is not null)
            {
                var enumValue = _formatter.ParseUserResponseCannotBeEmpty(args);
                await _onHandleUserSelectCannotBeNull.Invoke(enumValue);
            }
        }
        else
        {
            if(_onHandleUserSelect is not null)
            {
                var enumValue = _formatter.ParseUserResponse(args);
                await _onHandleUserSelect.Invoke(enumValue);
            }
        }

        await FinalizeCommand();
    }

    public EnumValueSelectionStepCommand(IMessageService<Message> messageService, string onCommandCreatedMessage, string noSelectButtonName, Func<Tuple<TEnum>?, Task> onHandleUserSelect, MessageBuilderOptions options, Func<TEnum, string> displayNameFormatter)
    {
        _messageService = messageService;

        _formatter = new(options, displayNameFormatter, onCommandCreatedMessage, noSelectButtonName);
        _noSelectButtonName = noSelectButtonName;
        _onHandleUserSelect = onHandleUserSelect;
    }

    public EnumValueSelectionStepCommand(IMessageService<Message> messageService, string onCommandCreatedMessage, Func<TEnum, Task> onHandleUserSelect, MessageBuilderOptions options, Func<TEnum, string?> displayNameFormatter)
    {
        _messageService = messageService;

        _formatter = new(options, displayNameFormatter, onCommandCreatedMessage);
        _onHandleUserSelectCannotBeNull = onHandleUserSelect;
    }

    public EnumValueSelectionStepCommand(IMessageService<Message> messageService, string onCommandCreatedMessage, Func<TEnum, Task> onHandleUserSelect, MessageBuilderOptions options, Func<TEnum, string?> displayNameFormatter, StepCommand next) : base(next)
    {
        _messageService = messageService;

        _formatter = new(options, displayNameFormatter, onCommandCreatedMessage);
        _onHandleUserSelectCannotBeNull = onHandleUserSelect;
    }
}