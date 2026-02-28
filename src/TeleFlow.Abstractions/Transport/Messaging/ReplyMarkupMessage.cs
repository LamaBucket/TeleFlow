using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Abstractions.Transport.Messaging;

public class ReplyMarkupMessage
{
    public required string Text { get; init; }

    public ParseMode ParseMode { get; init; } = ParseMode.None;

    public ReplyMarkupSpec Markup { get; init; } = new ReplyMarkupSpec.Remove(new());
}

public abstract record ReplyMarkupSpec
{
    private ReplyMarkupSpec() {}

    public sealed record Keyboard(ReplyKeyboardMarkup Markup) : ReplyMarkupSpec;
    public sealed record Remove(ReplyKeyboardRemove Markup) : ReplyMarkupSpec;
}