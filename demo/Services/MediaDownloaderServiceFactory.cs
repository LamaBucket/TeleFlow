using Telegram.Bot;
using TeleFlow.Services;

namespace demo.Services;

public class MediaDownloaderServiceFactory : IMediaDownloaderServiceFactory
{
    private readonly ITelegramBotClient _botClient;

    public IMediaDownloaderService CreateMediaDownloaderService()
    {
        return new MediaDownloaderService(_botClient);
    }

    public MediaDownloaderServiceFactory(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
}