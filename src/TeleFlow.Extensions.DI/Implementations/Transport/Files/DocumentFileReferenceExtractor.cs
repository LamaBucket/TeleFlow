using System;
using Telegram.Bot.Types;
using TeleFlow.Abstractions.Transport.Files;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Files;

public sealed class DocumentFileReferenceExtractor : IFileReferenceExtractor
{
    public bool TryExtract(Message message, out FileReference file)
    {
        file = default!;

        var d = message.Document;
        if (d is null)
            return false;

        // Если mime пустой — считаем документом (часто так бывает)
        if (!string.IsNullOrWhiteSpace(d.MimeType))
        {
            // не перехватываем медиа, пусть это заберут Image/Video/Audio экстракторы
            if (d.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return false;
            if (d.MimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)) return false;
            if (d.MimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)) return false;
        }

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