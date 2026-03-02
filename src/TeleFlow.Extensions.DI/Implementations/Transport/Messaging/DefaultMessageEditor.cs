using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageEditor : IMessageEditService
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public async Task<Message> Edit(int messageId, InlineMarkupMessage message)
    {
        Message? msg = null;

        try
        {
            msg = await _botClient.EditMessageTextAsync(_chatId, messageId, message.Text, parseMode: message.ParseMode);
        }
        catch (Exception ex) when (IsMessageNotModified(ex))
        {
            // ok, ничего не поменялось
        }

        try
        {
            msg = await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, replyMarkup: message.Markup);
        }
        catch (Exception ex) when (IsMessageNotModified(ex))
        {
            // ok
        }

        // Если оба edit'а были no-op, msg останется null — но это НЕ повод падать.
        // Вернём хотя бы "виртуальный успех" — например, получим текущее сообщение нельзя.
        // Поэтому: возвращай что-то другое или меняй контракт.
        if (msg is null)
            throw new InvalidOperationException("Edit resulted in no changes (message is not modified). Consider returning a Unit/void or tracking last message state.");

        return msg;
    }

    private static bool IsMessageNotModified(Exception ex)
    => ex is Telegram.Bot.Exceptions.ApiRequestException api
       && api.ErrorCode == 400
       && api.Message.Contains("message is not modified", StringComparison.OrdinalIgnoreCase);


    public DefaultMessageEditor(ITelegramBotClient botClient, long chatId)
    {
        _botClient = botClient;
        _chatId = chatId;
    }
}