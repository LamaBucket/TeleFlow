using Telegram.Bot.Types;
using TeleFlow.Abstractions.Transport.Files;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Files;

public sealed class ImageFileReferenceExtractor : IFileReferenceExtractor
{
    public bool TryExtract(Message message, out FileReference file)
    {
        file = default!;

        // 1) Photo (preferred)
        if (message.Photo is not null && message.Photo.Length > 0)
        {
            var best = message.Photo.OrderByDescending(p => p.FileSize ?? 0).First();

            file = new FileReference
            {
                ContentType = FileContentType.Photo,
                FileId = best.FileId,
                FileUniqueId = best.FileUniqueId,
                FileName = null,
                MimeType = null,
                DeclaredSizeBytes = best.FileSize,
                Width = best.Width,
                Height = best.Height,
                DurationSeconds = null
            };
            return true;
        }

        // 2) Document with image/*
        var d = message.Document;
        if (d is null)
            return false;

        if (string.IsNullOrWhiteSpace(d.MimeType))
            return false;

        if (!d.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return false;

        file = new FileReference
        {
            ContentType = FileContentType.Document,
            FileId = d.FileId,
            FileUniqueId = d.FileUniqueId,
            FileName = d.FileName,
            MimeType = d.MimeType,
            DeclaredSizeBytes = d.FileSize,
            Width = null,
            Height = null,
            DurationSeconds = null
        };
        return true;
    }
}