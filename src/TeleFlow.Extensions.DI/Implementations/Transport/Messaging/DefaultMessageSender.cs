using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageSender : IMessageSender
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public Task<Message> SendMessage(OutgoingMessage message)
        => _botClient.SendTextMessageAsync(_chatId, text: message.Text, replyMarkup: message.ReplyMarkup, parseMode: message.ParseMode);

    public DefaultMessageSender(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}