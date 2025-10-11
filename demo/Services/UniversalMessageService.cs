using LisBot.Common.Telegram.Models;
using Telegram.Bot;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo.Services;

public class UniversalMessageService
    : IMessageServiceWithEdit<Message>,
      IMessageService<string>,
      IMessageService<ImageMessageServiceMessage>
{
    private readonly long _chatId;

    private readonly ITelegramBotClient _botClient;

    private readonly InlineMarkupManager _inlineMarkupManager;

    public async Task<Message> EditMessage(int messageId, Message message)
    {
        await _inlineMarkupManager.RemovePreviousMessageReplyMarkup();

        var editedMessage = await _botClient.EditMessageTextAsync(_chatId, messageId, message?.Text ?? throw new ArgumentNullException(nameof(message)), replyMarkup: message.ReplyMarkup);

        _inlineMarkupManager.CheckMessage(editedMessage);

        return editedMessage;
    }

    public async Task<Message> SendMessage(Message message)
    {
        await _inlineMarkupManager.RemovePreviousMessageReplyMarkup();

        var sentMessage = await _botClient.SendTextMessageAsync(_chatId, message?.Text ?? throw new ArgumentNullException(nameof(message)), replyMarkup: message.ReplyMarkup);

        _inlineMarkupManager.CheckMessage(sentMessage);

        return sentMessage;
    }


    public async Task<Message> SendMessage(string message)
    {
        MessageBuilder builder = new();
        builder.WithText(message);

        return await SendMessage(builder.Build());
    }

    public async Task<Message> SendMessage(ImageMessageServiceMessage message)
    {
        if(!message.Images.Any())
            throw new ArgumentException("At least one image is required", nameof(message));

        await _inlineMarkupManager.RemovePreviousMessageReplyMarkup();
     
        if (message.Images.Count() == 1)
        {
            using var stream = new MemoryStream(message.Images.First());

            var sentMessage = await _botClient.SendPhotoAsync(
                chatId: _chatId,
                photo: InputFile.FromStream(stream),
                caption: message.Caption);

            _inlineMarkupManager.CheckMessage(sentMessage);

            return sentMessage;
        }
        else
        {
            var media = message.Images.Select((img, index) =>
                {
                    var stream = new MemoryStream(img);
                    var inputFile = InputFile.FromStream(stream, $"photo{index}.jpg");

                    return new InputMediaPhoto(inputFile);
                });

            var sentMessages = await _botClient.SendMediaGroupAsync(
                chatId: _chatId,
                media: media);

            // album returns multiple Messages — pick the first for further tracking
            var firstMessage = sentMessages.First();

            _inlineMarkupManager.CheckMessage(firstMessage);

            return firstMessage;
        }
    }

    public UniversalMessageService(ITelegramBotClient botClient, long chatId, InlineMarkupManager inlineMarkupManager)
    {
        _botClient = botClient;
        _chatId = chatId;
        _inlineMarkupManager = inlineMarkupManager;
    }
}
