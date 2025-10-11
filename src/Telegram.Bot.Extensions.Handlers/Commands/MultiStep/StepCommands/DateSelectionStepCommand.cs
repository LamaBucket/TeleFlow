using System.Threading.Tasks;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.ViewModels;
using Telegram.Bot.Extensions.Handlers.Services.InputValidators;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Commands.MultiStep.StepCommands;

public class DateSelectionStepCommand : StepCommandWithValidation
{
    private readonly DateSelectionMessageFormatter _viewsManager;

    private readonly DateSelectionStepCommandViewModel _viewModel;

    private readonly IMessageServiceWithEdit<Message> _messageService;

    private readonly Action<DateOnly> _onDateSelectedHandler;

    private readonly string _messageText;

    private readonly DateConstraint? _minDateConstraint;

    private readonly DateConstraint? _maxDateConstraint;

    private readonly Func<DateOnly, string>? _onDateSelectedUserMessage;


    public override async Task OnCommandCreated()
    {
        var message = await _messageService.SendMessage(new() { Text = "Hold on - We are working on it..." });

        _viewModel.ViewMessageId = message.MessageId;

        await RecreateMesssage();
    }

    protected override async Task HandleValidInput(Update args)
    {
        await _viewsManager.HandleUpdate(args);
    }

    private async Task OnViewChangeRequested(DateSelectionView newView)
    {
        _viewModel.CurrentView = newView;
        
        await RecreateMesssage();
    }

    private async Task<bool> OnDateSelected(DateOnly date)
    {
        if (_minDateConstraint is not null && date < _minDateConstraint.ConstraintValue)
        {
            MessageBuilder builder = new();
            builder.WithText(_minDateConstraint.ConstraintNotMetMessage);

            await _messageService.SendMessage(builder.Build());
            await RecreateMesssage();
            return false;
        }

        if (_maxDateConstraint is not null && date > _maxDateConstraint.ConstraintValue)
        {
            MessageBuilder builder = new();
            builder.WithText(_maxDateConstraint.ConstraintNotMetMessage);

            await _messageService.SendMessage(builder.Build());
            await RecreateMesssage();
            return false;
        }

        _viewModel.SelectedDate = date;

        await RecreateMesssage();

        return true;
    }

    private async Task OnDateConfirmed()
    {
        var messageToSend = _onDateSelectedUserMessage?.Invoke(_viewModel.SelectedDate);

        if (messageToSend is not null)
            await FinalizeDateSelection(messageToSend);


        _onDateSelectedHandler.Invoke(_viewModel.SelectedDate);

        await FinalizeCommand();
    }


    private async Task RecreateMesssage()
    {
        var message = _viewsManager.GenerateMessage(_viewModel.CurrentView, _messageText, _viewModel.SelectedDate);

        _viewModel.ViewMessageId = (await _messageService.EditMessage(_viewModel.ViewMessageId, message)).MessageId;
    }

    private async Task FinalizeDateSelection(string finalizeUserMessage)
    {
        MessageBuilder builder = new();
        builder.WithText(finalizeUserMessage);

        await _messageService.SendMessage(builder.Build());
    }


    public DateSelectionStepCommand(StepCommand? next,
                                    IUserInputValidator userInputValidator,
                                    IMessageServiceWithEdit<Message> messageService,
                                    Action<DateOnly> onDateSelectedHandler,
                                    string messageText,
                                    DateSelectionStepCommandViewModel viewModel,
                                    DateConstraint? minDateConstraint = null,
                                    DateConstraint? maxDateConstraint = null,
                                    Func<DateOnly, string>? onDateSelectedUserMessage = null) : base(next, userInputValidator)
    {
        _viewsManager = new();
        _viewModel = viewModel;

        _viewsManager.ViewChanged += OnViewChangeRequested;
        _viewsManager.DateSelected += OnDateSelected;

        _viewsManager.DateConfirmed += OnDateConfirmed;

        _messageService = messageService;
        _onDateSelectedHandler = onDateSelectedHandler;
        _messageText = messageText;
        _minDateConstraint = minDateConstraint;
        _maxDateConstraint = maxDateConstraint;
        _onDateSelectedUserMessage = onDateSelectedUserMessage;
    }
}


public class DateConstraint
{
    public DateOnly ConstraintValue { get; init; }

    public string ConstraintNotMetMessage { get; init; }

    public DateConstraint(DateOnly constraintValue, string constraintNotMetMessage)
    {
        ConstraintValue = constraintValue;
        ConstraintNotMetMessage = constraintNotMetMessage;
    }
}