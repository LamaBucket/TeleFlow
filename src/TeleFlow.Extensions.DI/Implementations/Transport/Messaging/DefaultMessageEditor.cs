using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageEditor : IMessageEditor
{
    private readonly ITelegramBotClient _botClient;

    private readonly IMessageEditorTemplateService? _templateService;

    private readonly long _chatId;

    public async Task Delete(int messageId)
    {
        await _botClient.DeleteMessageAsync(_chatId, messageId);
    }

    public async Task<Message> EditInlineKeyboard(int messageId, InlineKeyboardMarkup? markup)
    {
        if(_templateService is not null)
            markup = _templateService.UseTemplateOnInlineKeyboardEdit(markup);

        return await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, replyMarkup: markup);
    }

    public async Task<Message> EditText(int messageId, string text, ParseMode parseMode = ParseMode.None)
    {
        if(_templateService is not null)
            text = _templateService.UseTemplateOnTextEdit(text, parseMode);
            
        return await _botClient.EditMessageTextAsync(_chatId, messageId, text, parseMode: parseMode);
    }

    public DefaultMessageEditor(ITelegramBotClient botClient, long chatId, IMessageEditorTemplateService? templateService)
    {
        _botClient = botClient;
        _chatId = chatId;
        _templateService = templateService;
    }
}