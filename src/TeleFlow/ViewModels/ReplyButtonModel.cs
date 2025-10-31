using TeleFlow.ViewModels.CallbackQuery;

namespace TeleFlow.ViewModels;

public class ReplyButtonModel<T> where T : class
{
    public T InnerArgs { get; init; } 

    public string ButtonTitle { get; init; }

    public ReplyButtonModel(T innerArgs, string buttonTitle)
    {
        InnerArgs = innerArgs;
        ButtonTitle = buttonTitle;
    }
}