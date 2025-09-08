using Microsoft.AspNetCore.Razor.TagHelpers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public class MessageServiceWrapper : IMessageService<Message>, IMessageService<string>
{
    private readonly IMessageService<Message> _messageService;

    private readonly Action<MessageBuilder> _messageFormatter;

    public async Task<Message> SendMessage(Message message)
    {
        var messageBuilder = new MessageBuilder();

        _messageFormatter.Invoke(messageBuilder);

        message = ConcatMessages(message, messageBuilder.Build());

        return await _messageService.SendMessage(message);
    }

    public async Task<Message> SendMessage(string message)
    {
        var messageBuilder = new MessageBuilder();

        messageBuilder.WithText(message);

        return await SendMessage(messageBuilder.Build());
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

    public MessageServiceWrapper(IMessageService<Message> messageService, Action<MessageBuilder> messageFormatter)
    {
        _messageService = messageService;
        _messageFormatter = messageFormatter;
    }
}