using LisBot.Common.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class UniversalMessageServiceFactory
  : IMessageServiceFactory<Message>,
    IMessageServiceFactory<string>,
    IMessageServiceFactory<Tuple<string, KeyboardButton>>
{
    private readonly ITelegramBotClient _client;

    private readonly Dictionary<long, UniversalMessageService> _messageServiceCache;

    public UniversalMessageService Build(long chatId)
    {
        if (!_messageServiceCache.ContainsKey(chatId))
        {
            var messageService = new UniversalMessageService(_client, chatId);
            _messageServiceCache.Add(chatId, messageService);
        }

        return _messageServiceCache[chatId];
    }


    public IMessageService<Message> CreateMessageService(long chatId)
    {
        return Build(chatId);
    }

    IMessageService<string> IMessageServiceFactory<string>.CreateMessageService(long chatId)
    {
        return Build(chatId);
    }

    IMessageService<Tuple<string, KeyboardButton>> IMessageServiceFactory<Tuple<string, KeyboardButton>>.CreateMessageService(long chatId)
    {
        return Build(chatId);
    }


    public UniversalMessageServiceFactory(ITelegramBotClient client)
    {
        _client = client;
        _messageServiceCache = new();
    }
}