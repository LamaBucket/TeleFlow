using System.Threading.Tasks;
using LisBot.Common.Telegram.Services;
using LisBot.Common.Telegram.ViewModels;
using Telegram.Bot.Extensions.Handlers.Services.InputValidators;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep.StepCommands;

public class DateSelectionStepCommand : StepCommandWithValidation
{
    private readonly DateSelectionMessageFormatter _viewsManager;

    private readonly DateSelectionStepCommandViewModel _viewModel;

    private readonly IMessageServiceWithEdit<Message> _messageService;

    private readonly Action<DateOnly> _onDateSelectedHandler;

    private readonly string _messageText;


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

    private async Task OnDateSelected(DateOnly date)
    {
        _viewModel.SelectedDate = date;
        
        await RecreateMesssage();
    }

    private async Task OnDateConfirmed()
    {
        _onDateSelectedHandler.Invoke(_viewModel.SelectedDate);

        await FinalizeCommand();
    }

    private async Task RecreateMesssage()
    {
        var message = _viewsManager.GenerateMessage(_viewModel.CurrentView, _messageText, _viewModel.SelectedDate);

        _viewModel.ViewMessageId = (await _messageService.EditMessage(_viewModel.ViewMessageId, message)).MessageId;
    }

    public DateSelectionStepCommand(StepCommand? next,
                                    IUserInputValidator userInputValidator,
                                    IMessageServiceWithEdit<Message> messageService,
                                    Action<DateOnly> onDateSelectedHandler,
                                    string messageText,
                                    DateSelectionStepCommandViewModel viewModel) : base(next, userInputValidator)
    {
        _viewsManager = new();
        _viewModel = viewModel;

        _viewsManager.ViewChanged += OnViewChangeRequested;
        _viewsManager.DateSelected += OnDateSelected;

        _viewsManager.DateConfirmed += OnDateConfirmed;

        _messageService = messageService;
        _onDateSelectedHandler = onDateSelectedHandler;
        _messageText = messageText;
    }
}
