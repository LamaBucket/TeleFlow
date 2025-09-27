namespace Telegram.Bot.Extensions.Handlers.Services;

public interface IMediaDownloaderService
{
    Task<byte[]> DownloadFileAsync(string fileId);
}