using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Presentation.Builders;

public class ReplyKeyboardBuilder
{
    public static ReplyKeyboardRemove Remove() => new();
    
    private readonly List<List<KeyboardButton>> _rows = [];


    public ReplyKeyboardBuilder Row(params KeyboardButton[] buttons)
    {
        if (buttons.Length == 0) return this;
        _rows.Add([.. buttons]);
        return this;
    }

    public ReplyKeyboardBuilder ContactRequestButton(string text)
    {
        var btn = new KeyboardButton(text) { RequestContact = true };
        return Row(btn);
    }

    public ReplyKeyboardBuilder LocationRequestButton(string text)
    {
        var btn = new KeyboardButton(text) { RequestLocation = true };
        return Row(btn);
    }

    public ReplyKeyboardMarkup Build(
        bool resizeKeyboard = true,
        bool oneTimeKeyboard = true,
        bool selective = false,
        string? inputFieldPlaceholder = null
    )
    {
        return new ReplyKeyboardMarkup(_rows)
        {
            ResizeKeyboard = resizeKeyboard,
            OneTimeKeyboard = oneTimeKeyboard,
            Selective = selective,
            InputFieldPlaceholder = inputFieldPlaceholder
        };
    }
}