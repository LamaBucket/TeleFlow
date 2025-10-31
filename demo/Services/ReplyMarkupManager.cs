using Telegram.Bot;
using TeleFlow.Services.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class ReplyMarkupManager : IReplyMarkupManager
{
    private readonly long _chatId;

    private readonly ITelegramBotClient _botClient;

    public async Task ClearReplyButtons()
    {
        var deleteMessage = await _botClient.SendTextMessageAsync(_chatId, "We're clearing up, hang on a sec...", replyMarkup: new ReplyKeyboardRemove());
        await _botClient.DeleteMessageAsync(_chatId, deleteMessage.MessageId);
    }

    public async Task CreateReplyButtonMarkup(string message, ReplyKeyboardMarkup markup)
    {
        await _botClient.SendTextMessageAsync(_chatId, message, replyMarkup: markup);
    }

    public ReplyMarkupManager(long chatId, ITelegramBotClient botClient)
    {
        _chatId = chatId;
        _botClient = botClient;
    }

    
}
