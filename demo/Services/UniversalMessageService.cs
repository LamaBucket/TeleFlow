using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class UniversalMessageService
    : IMessageServiceWithEdit<Message>,
      IMessageService<string>
{
    private readonly DefaultMessageService _defaultMessageService;


    public async Task<Message> SendMessage(Message message)
    {
        return await _defaultMessageService.SendMessage(message);
    }

    public async Task<Message> EditMessage(int messageId, Message message)
    {
        return await _defaultMessageService.EditMessage(messageId, message);
    }

    public async Task<Message> SendMessage(string message)
    {
        var messageBuilder = new MessageBuilder();
        messageBuilder.WithTextLine(message);

        return await SendMessage(messageBuilder.Build());
    }


    public UniversalMessageService(ITelegramBotClient botClient, long chatId)
    {
        _defaultMessageService = new(botClient, chatId);
    }

}
