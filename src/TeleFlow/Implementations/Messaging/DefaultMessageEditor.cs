using TeleFlow.Models.Messaging;
using TeleFlow.Services.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Services.Defaults.Messaging;

public class DefaultMessageEditor : IMessageEditor
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public async Task Delete(int messageId)
    {
        await _botClient.DeleteMessageAsync(_chatId, messageId);
    }

    public async Task<Message> EditInlineKeyboard(int messageId, InlineKeyboardMarkup? markup)
    {
        return await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, replyMarkup: markup);
    }

    public async Task<Message> EditText(int messageId, string text, ParseMode parseMode = ParseMode.None)
    {
        return await _botClient.EditMessageTextAsync(_chatId, messageId, text, parseMode: parseMode);
    }

    public DefaultMessageEditor(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}