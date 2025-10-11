using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;

namespace demo.Services;

public class DemoInlineMarkupManagerFactory : InlineMarkupManagerFactory
{
    private readonly ITelegramBotClient _telegramBotClient;

    protected override InlineMarkupManager CreateInlineMarkupManager(long chatId)
    {
        return new DemoInlineMarkupManager(_telegramBotClient, chatId);
    }

    public DemoInlineMarkupManagerFactory(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }
}