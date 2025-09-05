using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class UniversalMessageServiceFactory
  : IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message>,
    IMessageServiceFactory<string>
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

    IMessageServiceWithEdit<Message> IMessageServiceFactory<IMessageServiceWithEdit<Message>, Message>.CreateMessageService(long chatId)
    {
        return Build(chatId);
    }

    IMessageService<string> IMessageServiceFactory<IMessageService<string>, string>.CreateMessageService(long chatId)
    {
        return Build(chatId);
    }

    public UniversalMessageServiceFactory(ITelegramBotClient client)
    {
        _client = client;
        _messageServiceCache = new();
    }
}