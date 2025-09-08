using System.Text;
using LisBot.Common.Telegram.ViewModels;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public class MessageBuilder
{
    private readonly StringBuilder _sb;

    private readonly MessageBuilderOptions _options;

    private readonly Queue<InlineKeyboardButton> _buttons;

    private readonly List<int> _buttonSeparators;


    private int _currentRowButtonCount;


    public Message Build()
    {
        var message = new Message();

        message.Text = _sb.ToString();
        message.ReplyMarkup = BuildMarkup();

        _sb.Clear();
        _buttons.Clear();

        return message;
    }

    private InlineKeyboardMarkup BuildMarkup()
    {
        List<List<InlineKeyboardButton>> markup = [];
        Queue<InlineKeyboardButton> buttons = new(_buttons);

        List<InlineKeyboardButton> currentLine = [];
        int count = 0;
        int sepIndex = 0;

        while (buttons.Count > 0)
        {
            currentLine.Add(buttons.Dequeue());
            count++;

            if (sepIndex < _buttonSeparators.Count && count == _buttonSeparators[sepIndex])
            {
                markup.Add(currentLine);
                currentLine = new();
                sepIndex++;
            }
        }

        if (currentLine.Count > 0)
            markup.Add(currentLine);

        return new(markup);
    }


    public MessageBuilder WithText(string text)
    {
        _sb.Append(text);
        return this;
    }

    public MessageBuilder WithTextLine(string text)
    {
        _sb.AppendLine(text);
        return this;
    }


    public MessageBuilder WithInlineButtonLine<T>(ReplyButtonModel<T> btnModel) where T : CallbackQueryViewModel
    {
        return WithInlineButton(btnModel).WithNewButtonLine();
    }

    public MessageBuilder WithInlineButton<T>(ReplyButtonModel<T> btnModel) where T : CallbackQueryViewModel
    {
        if(_currentRowButtonCount >= _options.ButtonInRowCount)
        {
            WithNewButtonLine();
        }

        var buttonText = btnModel.ButtonTitle;
        var buttonData = JsonConvert.SerializeObject(btnModel.InnerArgs);

        InlineKeyboardButton button = new(buttonText);
        button.CallbackData = buttonData;

        _buttons.Enqueue(button);
        _currentRowButtonCount += 1;

        return this;
    }

    public MessageBuilder WithNewButtonLine()
    {
        _buttonSeparators.Add(_buttons.Count);
        _currentRowButtonCount = 0;
        return this;
    }

    public MessageBuilder(MessageBuilderOptions? options = null)
    {
        _sb = new();
        _buttons = new();
        _buttonSeparators = [];
        _currentRowButtonCount = 0;

        _options = options ?? new MessageBuilderOptions();
    }
}