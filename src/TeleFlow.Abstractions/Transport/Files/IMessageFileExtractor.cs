using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Transport.Files;

public interface IFileReferenceExtractor
{
    bool TryExtract(Message message, out FileReference file);
}