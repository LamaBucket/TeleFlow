using System;
using Telegram.Bot.Types;
using TeleFlow.Abstractions.Transport.Files;

namespace TeleFlow.DependencyInjection.Implementations.Transport.Files;

public sealed class VideoFileReferenceExtractor : IFileReferenceExtractor
{
    public bool TryExtract(Message message, out FileReference file)
    {
        file = default!;

        // 1) Native video
        var v = message.Video;
        if (v is not null)
        {
            file = new FileReference
            {
                ContentType = FileContentType.Video,
                FileId = v.FileId,
                FileUniqueId = v.FileUniqueId,
                FileName = null,
                MimeType = v.MimeType,
                DeclaredSizeBytes = v.FileSize,
                Width = v.Width,
                Height = v.Height,
                DurationSeconds = v.Duration
            };
            return true;
        }

        // 2) Document with video/*
        var d = message.Document;
        if (d is null)
            return false;

        if (string.IsNullOrWhiteSpace(d.MimeType))
            return false;

        if (!d.MimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
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