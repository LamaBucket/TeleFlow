using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class UniversalMessageService : TemplatedMessageService, IMessageService<string>, IMessageService<Tuple<string, KeyboardButton>>
{
    private readonly long _chatId;

    private readonly ITelegramBotClient _botClient;


    protected override async Task RemoveReplyMarkup(int messageId)
    {
        await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, null);
    }

    protected override async Task<Message> SendMessage(string messageText, IReplyMarkup? replyMarkup)
    {
        return await _botClient.SendTextMessageAsync(_chatId, messageText, replyMarkup: replyMarkup);
    }

    protected override async Task<Message> EditMessage(int messageId, string messageText, IReplyMarkup? replyMarkup)
    {
        if (replyMarkup is InlineKeyboardMarkup inlineMarkup)
        {
            return await _botClient.EditMessageTextAsync(_chatId, messageId, messageText, replyMarkup: inlineMarkup);
        }
        else
        {
            return await _botClient.EditMessageTextAsync(_chatId, messageId, messageText);
        }
    }




    public async Task<Message> SendMessage(string message)
    {
        var msgBuilder = new MessageBuilder();
        msgBuilder.WithText(message);

        return await SendMessage(msgBuilder.Build());
    }

    public async Task<Message> EditMessage(int messageId, string message)
    {
        var msgBuilder = new MessageBuilder();
        msgBuilder.WithText(message);

        return await EditMessage(messageId, msgBuilder.Build());
    }


    public async Task<Message> SendMessage(Tuple<string, KeyboardButton> message)
    {
        return await _botClient.SendTextMessageAsync(_chatId, message.Item1, replyMarkup: new ReplyKeyboardMarkup(message.Item2));
    }

    public async Task<Message> EditMessage(int messageId, Tuple<string, KeyboardButton> message)
    {
        throw new Exception("TEMP - NOT AVAILABLE");
    }

    public UniversalMessageService(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}
