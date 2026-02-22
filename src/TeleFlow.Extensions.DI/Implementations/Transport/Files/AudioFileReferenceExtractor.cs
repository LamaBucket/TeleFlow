using System;
using Telegram.Bot.Types;
using TeleFlow.Abstractions.Transport.Files;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Files;

public sealed class AudioFileReferenceExtractor : IFileReferenceExtractor
{
    public bool TryExtract(Message message, out FileReference file)
    {
        file = default!;

        // 1) Voice (voice messages)
        var voice = message.Voice;
        if (voice is not null)
        {
            file = new FileReference
            {
                ContentType = FileContentType.Voice,
                FileId = voice.FileId,
                FileUniqueId = voice.FileUniqueId,
                FileName = null,
                MimeType = voice.MimeType,
                DeclaredSizeBytes = voice.FileSize,
                Width = null,
                Height = null,
                DurationSeconds = voice.Duration
            };
            return true;
        }

        // 2) Audio (music/audio files)
        var audio = message.Audio;
        if (audio is not null)
        {
            file = new FileReference
            {
                ContentType = FileContentType.Audio,
                FileId = audio.FileId,
                FileUniqueId = audio.FileUniqueId,
                FileName = audio.FileName,
                MimeType = audio.MimeType,
                DeclaredSizeBytes = audio.FileSize,
                Width = null,
                Height = null,
                DurationSeconds = audio.Duration
            };
            return true;
        }

        // 3) Document with audio/*
        var d = message.Document;
        if (d is null)
            return false;

        if (string.IsNullOrWhiteSpace(d.MimeType))
            return false;

        if (!d.MimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
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