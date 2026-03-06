using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageDeleteService : IMessageDeleteService
{
    private readonly long _chatId;

    private readonly ITelegramBotClient _botClient;

    public async Task Delete(int messageId)
    {
        await _botClient.DeleteMessageAsync(_chatId, messageId);
    }

    public DefaultMessageDeleteService(long chatId, ITelegramBotClient botClient)
    {
        _chatId = chatId;
        _botClient = botClient;
    }
}