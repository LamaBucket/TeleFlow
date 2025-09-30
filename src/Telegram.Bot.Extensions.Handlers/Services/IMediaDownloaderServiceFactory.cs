namespace Telegram.Bot.Extensions.Handlers.Services;

public interface IMediaDownloaderServiceFactory
{
    IMediaDownloaderService CreateMediaDownloaderService();
}