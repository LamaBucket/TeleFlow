using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Abstractions.Transport.Messaging;

public class InlineMarkupMessage
{
    public static InlineMarkupMessage CreateTextMessage(string text) => new() { Text = text };


    public required string Text { get; init; }

    public ParseMode ParseMode { get; init; } = ParseMode.None;

    public InlineKeyboardMarkup? Markup { get; init; } = null;

}