using Telegram.Bot;
using TeleFlow.Services.Messaging;

namespace demo.Services;

public class DemoInlineMarkupManager : InlineMarkupManager
{
    private readonly ITelegramBotClient _telegramBotClient;

    private readonly long _chatId;

    protected override async Task RemoveReplyMarkup(int messageId)
    {
        await _telegramBotClient.EditMessageReplyMarkupAsync(_chatId, messageId);
    }

    public DemoInlineMarkupManager(ITelegramBotClient telegramBotClient, long chatId)
    {
        _telegramBotClient = telegramBotClient;
        _chatId = chatId;
    }
}