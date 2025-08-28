using LisBot.Common.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class UniversalMessageService : TemplatedMessageService, IMessageService<string>, IMessageService<Tuple<string, KeyboardButton>>
{
    private readonly long _chatId;

    private readonly ITelegramBotClient _botClient;


    protected override void RemoveReplyMarkup(int messageId)
    {
        _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, null);
    }

    protected override async Task<Message> SendMessage(string messageText, IReplyMarkup? replyMarkup)
    {
        return await _botClient.SendTextMessageAsync(_chatId, messageText, replyMarkup: replyMarkup);
    }




    public async Task<Message> SendMessage(string message)
    {
        var msgBuilder = new MessageBuilder();
        msgBuilder.WithText(message);

        return await SendMessage(msgBuilder.Build());
    }

    public async Task<Message> SendMessage(Tuple<string, KeyboardButton> message)
    {
        return await _botClient.SendTextMessageAsync(_chatId, message.Item1, replyMarkup: new ReplyKeyboardMarkup(message.Item2));
    }

    public UniversalMessageService(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}
