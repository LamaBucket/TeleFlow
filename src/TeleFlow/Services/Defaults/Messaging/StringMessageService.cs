using TeleFlow.Services.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleFlow.Services.Defaults.Messaging;

public class StringMessageService : IMessageService<string>
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public async Task<Message> SendMessage(string message)
    {
        return await _botClient.SendTextMessageAsync(_chatId, message);
    }

    public StringMessageService(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}