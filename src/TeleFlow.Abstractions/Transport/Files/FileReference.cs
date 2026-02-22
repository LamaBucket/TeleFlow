namespace TeleFlow.Abstractions.Transport.Files;

public sealed class FileReference
{
    public required FileContentType ContentType { get; init; }

    public required string FileId { get; init; }

    public required string FileUniqueId { get; init; }

    public string? FileName { get; init; }

    public string? MimeType { get; init; }

    public long? DeclaredSizeBytes { get; init; }

    public int? Width { get; init; }

    public int? Height { get; init; }

    public int? DurationSeconds { get; init; }
}