using Telegram.Bot.Extensions.Handlers.Services;

namespace Telegram.Bot.Extensions.Handlers.ViewModels;

public class DateSelectionStepCommandViewModel
{
    public DateOnly SelectedDate { get; set; }

    public DateSelectionView CurrentView { get; set; }


    public int ViewMessageId
    {
        get => _viewMessageId ?? throw new ArgumentNullException(nameof(_viewMessageId));
        set => _viewMessageId = value;
    }

    private int? _viewMessageId;


    public DateSelectionStepCommandViewModel(DateOnly selectedDate, DateSelectionView currentView = DateSelectionView.DaySelection)
    {
        CurrentView = DateSelectionView.DaySelection;
        SelectedDate = selectedDate;
        CurrentView = currentView;
    }
}