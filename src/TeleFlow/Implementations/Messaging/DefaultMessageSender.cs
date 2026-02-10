using TeleFlow.Models.Messaging;
using TeleFlow.Services.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleFlow.Services.Defaults.Messaging;

public class DefaultMessageSender : IMessageSender
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public async Task<Message> SendMessage(OutgoingMessage message)
    {
        return await _botClient.SendTextMessageAsync(_chatId, text: message.Text, replyMarkup: message.ReplyMarkup, parseMode: message.ParseMode);
    }

    public DefaultMessageSender(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}