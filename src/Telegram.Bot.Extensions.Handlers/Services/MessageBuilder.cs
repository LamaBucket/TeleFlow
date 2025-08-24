using System.Collections.Generic;
using System.Text;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.ViewModels;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Services;

public class MessageBuilder
{
    private readonly StringBuilder _sb;

    private readonly MessageBuilderOptions _options;

    private readonly Queue<InlineKeyboardButton> _buttons;

    public Message Build()
    {
        var message = new Message();

        message.Text = _sb.ToString();
        message.ReplyMarkup = BuildMarkup();

        if(_options.ClearOnBuild)
        {
            _sb.Clear();
            _buttons.Clear();
        }

        return message;
    }

    private InlineKeyboardMarkup BuildMarkup()
    {
        List<List<InlineKeyboardButton>> markup = new();

        Queue<InlineKeyboardButton> buttons = new(_buttons);

        while(buttons.Count != 0)
        {
            int buttonColumn = 0;
            List<InlineKeyboardButton> currentLine = new();

            while(buttonColumn < _options.ButtonInRowCount && buttons.Count != 0)
            {
                currentLine.Add(buttons.Dequeue());

                buttonColumn += 1;
            }

            markup.Add(currentLine);
        }

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

    public MessageBuilder WithInlineButton<T>(ReplyButtonModel<T> btnModel) where T : CallbackQueryViewModel
    {
        var buttonText = btnModel.ButtonTitle;
        var buttonData = JsonConvert.SerializeObject(btnModel.InnerArgs);

        InlineKeyboardButton button = new(buttonText);
        button.CallbackData = buttonData;

        _buttons.Enqueue(button);

        return this;
    }

    public MessageBuilder(MessageBuilderOptions? options = null)
    {
        _sb = new();
        _buttons = new();
        _options = options ?? new();
    }
}