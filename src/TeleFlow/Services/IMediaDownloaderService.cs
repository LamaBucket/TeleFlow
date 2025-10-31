namespace TeleFlow.Services;

public interface IMediaDownloaderService
{
    Task<byte[]> DownloadFileAsync(string fileId);
}