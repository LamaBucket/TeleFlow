using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageSender : IMessageSender
{
    private readonly ITelegramBotClient _botClient;

    private readonly IMessageSenderTemplateService? _templateService;

    private readonly long _chatId;

    public async Task<Message> SendMessage(OutgoingMessage message)
    {
        if(_templateService is not null)
            message = _templateService.UseTemplate(message);

        return await _botClient.SendTextMessageAsync(_chatId, text: message.Text, replyMarkup: message.ReplyMarkup, parseMode: message.ParseMode);
    }

    public DefaultMessageSender(ITelegramBotClient botClient, long chatId, IMessageSenderTemplateService? templateService)
    {
        _botClient = botClient;
        _chatId = chatId;
        _templateService = templateService;
    }
}