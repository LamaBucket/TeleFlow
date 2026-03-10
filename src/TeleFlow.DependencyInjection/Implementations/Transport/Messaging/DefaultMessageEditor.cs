using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.DependencyInjection.Implementations.Transport.Messaging;

public class DefaultMessageEditor : IMessageEditService
{
    private readonly ITelegramBotClient _botClient;

    private readonly long _chatId;

    public async Task Edit(int messageId, InlineMarkupMessage message)
    {
        try
        {
            await _botClient.EditMessageTextAsync(_chatId, messageId, message.Text, parseMode: message.ParseMode);
        }
        catch (Exception ex) when (IsMessageNotModified(ex))
        {
            // Ok, nothing changed
        }

        try
        {
            await _botClient.EditMessageReplyMarkupAsync(_chatId, messageId, replyMarkup: message.Markup);
        }
        catch (Exception ex) when (IsMessageNotModified(ex))
        {
            // Ok, nothing changed
        }
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