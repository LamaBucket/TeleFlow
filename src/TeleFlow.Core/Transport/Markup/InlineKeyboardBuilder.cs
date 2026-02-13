using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Transport.Markup;

public sealed class InlineKeyboardBuilder
{
    private readonly List<List<InlineKeyboardButton>> _rows = [];

    public InlineKeyboardBuilder Row(params InlineKeyboardButton[] buttons)
    {
        if (buttons.Length == 0) return this;
        _rows.Add([.. buttons]);
        return this;
    }

    public InlineKeyboardBuilder ButtonCallback(string text, string data)
        => Button(InlineKeyboardButton.WithCallbackData(text, data));

    public InlineKeyboardBuilder ButtonUrl(string text, string url)
        => Button(InlineKeyboardButton.WithUrl(text, url));

    public InlineKeyboardBuilder ButtonSwitchInlineQuery(string text, string query)
        => Button(InlineKeyboardButton.WithSwitchInlineQuery(text, query));

    public InlineKeyboardBuilder ButtonSwitchInlineQueryCurrentChat(string text, string query)
        => Button(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(text, query));

    public InlineKeyboardBuilder ButtonPay(string text)
        => Button(InlineKeyboardButton.WithPay(text));

    public InlineKeyboardBuilder ButtonLogin(string text, LoginUrl loginUrl)
        => Button(InlineKeyboardButton.WithLoginUrl(text, loginUrl));

    public InlineKeyboardBuilder ButtonWebApp(string text, WebAppInfo webAppInfo)
        => Button(InlineKeyboardButton.WithWebApp(text, webAppInfo));

    public InlineKeyboardBuilder Button(InlineKeyboardButton button)
    {
        if (_rows.Count == 0) _rows.Add(new List<InlineKeyboardButton>());
        _rows[^1].Add(button);
        return this;
    }

    public InlineKeyboardBuilder NewRow()
    {
        _rows.Add(new List<InlineKeyboardButton>());
        return this;
    }

    public InlineKeyboardMarkup Build()
        => new InlineKeyboardMarkup(_rows);

    public static InlineKeyboardMarkup SingleRow(params InlineKeyboardButton[] buttons)
        => new InlineKeyboardBuilder().Row(buttons).Build();

    public static InlineKeyboardMarkup SingleButtonCallback(string text, string data)
        => new InlineKeyboardBuilder().Row(InlineKeyboardButton.WithCallbackData(text, data)).Build();
}