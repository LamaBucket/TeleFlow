using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public abstract class TemplatedMessageService : IMessageServiceWithEdit<Message>
{
    private Message? _lastMessageWithInlineMarkup;

    public async Task<Message> SendMessage(Message message)
    {
        var messageText = message.Text ?? throw new ArgumentNullException(nameof(message.Text));
        var messageReplyMarkup = message.ReplyMarkup;

        await RemovePreviousMessageReplyMarkup();

        var sentMessage = await SendMessage(messageText, messageReplyMarkup);


        bool messageHasReplyMarkup = messageReplyMarkup?.InlineKeyboard.Any() ?? false;

        if (messageHasReplyMarkup)
            _lastMessageWithInlineMarkup = sentMessage;

        return sentMessage;
    }

    public async Task<Message> EditMessage(int messageId, Message message)
    {
        var messageText = message.Text ?? throw new ArgumentNullException(nameof(message.Text));
        var messageReplyMarkup = message.ReplyMarkup;

        await RemovePreviousMessageReplyMarkup();

        var editedMessage = await EditMessage(messageId, messageText, messageReplyMarkup);

        bool messageHasReplyMarkup = messageReplyMarkup?.InlineKeyboard.Any() ?? false;

        if (messageHasReplyMarkup)
            _lastMessageWithInlineMarkup = editedMessage;

        return editedMessage;
    }


    private async Task RemovePreviousMessageReplyMarkup()
    {
        if (_lastMessageWithInlineMarkup is not null)
        {
            await RemoveReplyMarkup(_lastMessageWithInlineMarkup.MessageId);
        }

        _lastMessageWithInlineMarkup = null;
    }


    protected abstract Task RemoveReplyMarkup(int messageId);

    protected abstract Task<Message> SendMessage(string messageText, InlineKeyboardMarkup? replyMarkup);

    protected abstract Task<Message> EditMessage(int messageId, string messageText, InlineKeyboardMarkup? replyMarkup);
}
