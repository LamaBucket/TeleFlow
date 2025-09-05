using LisBot.Common.Telegram.Services;

namespace LisBot.Common.Telegram.ViewModels;

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