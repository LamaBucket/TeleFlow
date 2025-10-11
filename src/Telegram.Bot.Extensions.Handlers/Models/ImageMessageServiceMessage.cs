namespace Telegram.Bot.Extensions.Handlers.Models;

public class ImageMessageServiceMessage
{
    public IEnumerable<byte[]> Images { get; init; }

    public string Caption { get; init; }

    public ImageMessageServiceMessage(IEnumerable<byte[]> images, string caption)
    {
        Images = images;
        Caption = caption;
    }
}