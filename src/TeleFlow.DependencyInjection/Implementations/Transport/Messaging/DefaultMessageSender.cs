using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.DependencyInjection.Implementations.Transport.Messaging;

public class DefaultMessageSender : IMessageSendService
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public Task<Message> SendMessage(InlineMarkupMessage message)
        => _botClient.SendTextMessageAsync(_chatId, text: message.Text, replyMarkup: message.Markup, parseMode: message.ParseMode);

    public async Task<Message> SendMessage(ReplyMarkupMessage message)
    {
        IReplyMarkup? messageMarkup = null;

        if(message.Markup is ReplyMarkupSpec.Keyboard keyboard)
            messageMarkup = keyboard.Markup;
        
        if(message.Markup is ReplyMarkupSpec.Remove remove)
            messageMarkup = remove.Markup;

        if(messageMarkup is null)
            throw new Exception();

        return await _botClient.SendTextMessageAsync(_chatId, text: message.Text, replyMarkup: messageMarkup, parseMode: message.ParseMode);   
    }

    public DefaultMessageSender(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}