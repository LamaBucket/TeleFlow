using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Models.Messaging;

public class OutgoingMessage
{
    public static OutgoingMessage CreateTextMessage(string text) => new() { Text = text };


    public required string Text { get; init; }

    public IReplyMarkup? ReplyMarkup { get; init; } = null;

    public ParseMode ParseMode { get; init; } = ParseMode.None;

    public OutgoingMessage()
    {
    }
}