using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public abstract class TemplatedMessageService : IMessageService<Message>
{
    private Message? _lastMessageWithInlineMarkup;

    public async Task<Message> SendMessage(Message message)
    {
        var messageText = message.Text ?? throw new ArgumentNullException(nameof(message.Text));
        var messageReplyMarkup = message.ReplyMarkup;

        RemovePreviousMessageReplyMarkup();

        var sentMessage = await SendMessage(messageText, messageReplyMarkup);

        if(messageReplyMarkup is not null)
            _lastMessageWithInlineMarkup = sentMessage;

        return sentMessage;
    }

    private void RemovePreviousMessageReplyMarkup()
    {
        if(_lastMessageWithInlineMarkup is not null)
        {
            RemoveReplyMarkup(_lastMessageWithInlineMarkup.MessageId);
        }
    }

    protected abstract void RemoveReplyMarkup(int messageId);

    protected abstract Task<Message> SendMessage(string messageText, IReplyMarkup? replyMarkup);


}
