using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Transport.Messaging;

public interface IMessageEditService
{
    Task<Message> Edit(int messageId, InlineMarkupMessage message);
}