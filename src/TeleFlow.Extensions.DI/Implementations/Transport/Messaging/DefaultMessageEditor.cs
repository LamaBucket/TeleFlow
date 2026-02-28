using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageEditor : IMessageEditor
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public Task Delete(int messageId)
        => _botClient.DeleteMessageAsync(_chatId, messageId);

    public Task<Message> EditInlineKeyboard(int messageId, InlineKeyboardMarkup? markup)
        => _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, replyMarkup: markup);

    public Task<Message> EditText(int messageId, string text, ParseMode parseMode = ParseMode.None)
        => _botClient.EditMessageTextAsync(_chatId, messageId, text, parseMode: parseMode);

    public DefaultMessageEditor(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}