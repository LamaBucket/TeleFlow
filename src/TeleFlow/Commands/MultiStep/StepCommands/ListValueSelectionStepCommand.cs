using TeleFlow.Services.InputValidators;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;

namespace TeleFlow.Commands.MultiStep.StepCommands;

public class ListValueSelectionStepCommand<TEnumerable> : StepCommandWithValidation where TEnumerable : class
{
    private readonly IMessageService<Message> _messageService;

    private ListValueSelectionMessageFormatter<TEnumerable> Formatter => _formatter ?? throw new ArgumentNullException(nameof(_formatter));

    private ListValueSelectionMessageFormatter<TEnumerable>? _formatter;

    private readonly MessageBuilderOptions _options;

    private readonly Func<Task<IEnumerable<TEnumerable>>> _valueProvider;

    private readonly Func<TEnumerable, string> _displayNameProvider;

    private readonly string _onCommandCreatedMessage;



    private readonly string? _noSelectButtonName;


    private readonly Func<TEnumerable, Task>? _onHandleUserSelectCannotBeNull;

    private readonly Func<TEnumerable?, Task>? _onHandleUserSelect;


    public override async Task OnCommandCreated()
    {
        await SetupFormatter();

        await _messageService.SendMessage(Formatter.GenerateMessage());
    }

    private async Task SetupFormatter()
    {
        var values = await _valueProvider.Invoke();

        _formatter = new(_options, values, _displayNameProvider, _onCommandCreatedMessage);
    }

    protected override async Task HandleValidInput(Update args)
    {
        if (_noSelectButtonName is null)
        {
            if (_onHandleUserSelectCannotBeNull is not null)
            {
                var enumValue = Formatter.ParseUserResponseCannotBeEmpty(args);
                await _onHandleUserSelectCannotBeNull.Invoke(enumValue);
            }
        }
        else
        {
            if (_onHandleUserSelect is not null)
            {
                var enumValue = Formatter.ParseUserResponse(args);
                await _onHandleUserSelect.Invoke(enumValue);
            }
        }

        await FinalizeCommand();
    }

    public ListValueSelectionStepCommand(IMessageService<Message> messageService,
                                         string onCommandCreatedMessage,
                                         Func<TEnumerable, Task> onHandleUserSelect,
                                         MessageBuilderOptions options,
                                         IEnumerable<TEnumerable> values,
                                         Func<TEnumerable, string> displayNameFormatter,
                                         StepCommand? next,
                                         IUserInputValidator? userInputValidator = null) : base(next, userInputValidator)
    {
        _messageService = messageService;

        _onHandleUserSelectCannotBeNull = onHandleUserSelect;
        
        _options = options;
        _valueProvider = async () => { return values; };
        _displayNameProvider = displayNameFormatter;
        _onCommandCreatedMessage = onCommandCreatedMessage;
    }

    public ListValueSelectionStepCommand(IMessageService<Message> messageService,
                                         string onCommandCreatedMessage,
                                         Func<TEnumerable, Task> onHandleUserSelect,
                                         MessageBuilderOptions options,
                                         Func<Task<IEnumerable<TEnumerable>>> values,
                                         Func<TEnumerable, string> displayNameFormatter,
                                         StepCommand? next,
                                         IUserInputValidator? userInputValidator = null) : base(next, userInputValidator)
    {
        _messageService = messageService;

        _onHandleUserSelectCannotBeNull = onHandleUserSelect;
        
        _options = options;
        _valueProvider = values;
        _displayNameProvider = displayNameFormatter;
        _onCommandCreatedMessage = onCommandCreatedMessage;
    }
}