using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class DefaultMessageService : TemplatedMessageService
{
    private readonly long _chatId;

    private readonly ITelegramBotClient _botClient;


    protected override async Task<Message> SendMessage(string messageText, InlineKeyboardMarkup? replyMarkup)
    {
        return await _botClient.SendTextMessageAsync(_chatId, messageText, replyMarkup: replyMarkup);
    }

    protected override async Task<Message> EditMessage(int messageId, string messageText, InlineKeyboardMarkup? replyMarkup)
    {
        return await _botClient.EditMessageTextAsync(_chatId, messageId, messageText, replyMarkup: replyMarkup);
    }


    protected override async Task RemoveReplyMarkup(int messageId)
    {
        await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, null);
    }


    public DefaultMessageService(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}