using Telegram.Bot.Types;

namespace TeleFlow.Services.Messaging;

public abstract class InlineMarkupManager
{
    private Message? _lastMessageWithInlineMarkup;

    public void CheckMessage(Message message)
    {
        var messageReplyMarkup = message.ReplyMarkup;
        bool messageHasReplyMarkup = messageReplyMarkup?.InlineKeyboard.Any() ?? false;

        if (messageHasReplyMarkup)
            _lastMessageWithInlineMarkup = message;
    }

    public async Task RemovePreviousMessageReplyMarkup()
    {
        if (_lastMessageWithInlineMarkup is not null)
        {
            await RemoveReplyMarkup(_lastMessageWithInlineMarkup.MessageId);
        }

        _lastMessageWithInlineMarkup = null;
    }

    protected abstract Task RemoveReplyMarkup(int messageId);
    
    
    protected InlineMarkupManager()
    {

    }
}