using Microsoft.AspNetCore.Razor.TagHelpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Services.Messaging;

public class MessageServiceWrapper : IMessageServiceWithEdit<Message>, IMessageService<string>
{
    private readonly IMessageServiceWithEdit<Message> _messageService;

    private readonly Action<MessageBuilder> _messageFormatter;


    private readonly InlineMarkupManager _inlineMarkupManager;


    public async Task<Message> SendMessage(Message message)
    {
        await _inlineMarkupManager.RemovePreviousMessageReplyMarkup();

        var messageBuilder = new MessageBuilder();

        _messageFormatter.Invoke(messageBuilder);

        message = ConcatMessages(message, messageBuilder.Build());

        var sentMessage = await _messageService.SendMessage(message);

        _inlineMarkupManager.CheckMessage(sentMessage);

        return sentMessage;
    }

    public async Task<Message> EditMessage(int messageId, Message message)
    {
        await _inlineMarkupManager.RemovePreviousMessageReplyMarkup();

        var messageBuilder = new MessageBuilder();

        _messageFormatter.Invoke(messageBuilder);

        message = ConcatMessages(message, messageBuilder.Build());

        var editedMessage = await _messageService.EditMessage(messageId, message);

        _inlineMarkupManager.CheckMessage(editedMessage);

        return editedMessage;
    }


    public async Task<Message> SendMessage(string message)
    {
        await _inlineMarkupManager.RemovePreviousMessageReplyMarkup();

        var messageBuilder = new MessageBuilder();

        messageBuilder.WithText(message);

        var sentMessage = await SendMessage(messageBuilder.Build());

        _inlineMarkupManager.CheckMessage(sentMessage);

        return sentMessage;
    }


    private static Message ConcatMessages(Message message1, Message message2)
    {
        var text = message1.Text + message2.Text;
        var replyMarkup = new List<IEnumerable<InlineKeyboardButton>>();

        foreach (var markup in message1.ReplyMarkup?.InlineKeyboard ?? [])
        {
            replyMarkup.Add(markup);
        }

        foreach (var markup in message2.ReplyMarkup?.InlineKeyboard ?? [])
        {
            replyMarkup.Add(markup);
        }

        message1.Text = text;
        message1.ReplyMarkup = new(replyMarkup);

        return message1;
    }


    public MessageServiceWrapper(IMessageServiceWithEdit<Message> messageService, Action<MessageBuilder> messageFormatter, InlineMarkupManager inlineMarkupManager)
    {
        _messageService = messageService;
        _messageFormatter = messageFormatter;
        _inlineMarkupManager = inlineMarkupManager;
    }
}