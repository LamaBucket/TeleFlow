
using Telegram.Bot;
using TeleFlow.Services;

namespace demo.Services;

public class MediaDownloaderService : IMediaDownloaderService
{
    private readonly ITelegramBotClient _botClient;

    public async Task<byte[]> DownloadFileAsync(string fileId)
    {
        using var memoryStream = new MemoryStream();

        await _botClient.GetInfoAndDownloadFileAsync(fileId, memoryStream);

        var bytes = memoryStream.ToArray();

        return bytes;
    }

    public MediaDownloaderService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
}