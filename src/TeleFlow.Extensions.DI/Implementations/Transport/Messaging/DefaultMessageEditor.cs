using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageEditor : IMessageEditService
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public async Task<Message> Edit(int messageId, InlineMarkupMessage message)
    {
        Message? msg = null;
        try
        {
            msg = await _botClient.EditMessageTextAsync(_chatId, messageId, message.Text, parseMode: message.ParseMode);   
        }
        catch
        {
            Console.WriteLine("oops");
        }

        try
        {
            msg = await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, replyMarkup: message.Markup);   
        }
        catch
        {
            Console.WriteLine("oops");
        }

        if(msg is null)
            throw new Exception();

        return msg;
    }

    public DefaultMessageEditor(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}