using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Extensions.DependencyInjection.Polling.Configuration.Internal;

internal static class TeleFlowPollingDefaultExceptionHandlers
{
    internal static void OnUpdateExceptionDefault(ILogger logger, Update update, Exception ex)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            if (ex is OperationCanceledException or TaskCanceledException)
            {
                logger.LogDebug(ex, "Update handling cancelled. UpdateId={UpdateId} Type={Type}", update.Id, update.Type);
                return;
            }
        }

        if (logger.IsEnabled(LogLevel.Error))
        {
            var (chatId, userId, messageId) = ExtractIds(update);

            logger.LogError(ex,
                            "Update handling failed. UpdateId={UpdateId} Type={Type} ChatId={ChatId} UserId={UserId} MessageId={MessageId}",
                            update.Id,
                            update.Type,
                            chatId,
                            userId,
                            messageId);
        }
    }

    internal static void OnHandlerExceptionDefault(ILogger logger, HandleErrorSource source, Exception ex)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            if (ex is OperationCanceledException or TaskCanceledException)
            {
                logger.LogDebug(ex, "Telegram receiver cancelled. Source={Source}", source);
                return;
            }
        }

        if (logger.IsEnabled(LogLevel.Error))
        {
            logger.LogError(ex,
                            "Telegram receiver error. Source={Source} ExceptionType={ExceptionType}",
                            source,
                            ex.GetType().FullName);
        }
    }

    private static (long? ChatId, long? UserId, int? MessageId) ExtractIds(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
            {
                var m = update.Message;
                if (m is null) return (null, null, null);

                long? chatId = m.Chat?.Id;
                long? userId = m.From?.Id;
                int? msgId = m.MessageId;

                return (chatId, userId, msgId);
            }

            case UpdateType.EditedMessage:
            {
                var m = update.EditedMessage;
                if (m is null) return (null, null, null);

                long? chatId = m.Chat?.Id;
                long? userId = m.From?.Id;
                int? msgId = m.MessageId;

                return (chatId, userId, msgId);
            }

            case UpdateType.CallbackQuery:
            {
                var cq = update.CallbackQuery;
                if (cq is null) return (null, null, null);

                long? chatId = cq.Message?.Chat?.Id;
                long? userId = cq.From?.Id;
                int? msgId = cq.Message?.MessageId;

                return (chatId, userId, msgId);
            }

            case UpdateType.InlineQuery:
            {
                var iq = update.InlineQuery;
                if (iq is null) return (null, null, null);

                long? chatId = null;
                long? userId = iq.From?.Id;
                int? msgId = null;

                return (chatId, userId, msgId);
            }

            case UpdateType.ChosenInlineResult:
            {
                var cir = update.ChosenInlineResult;
                if (cir is null) return (null, null, null);

                long? chatId = null;
                long? userId = cir.From?.Id;
                int? msgId = null;

                return (chatId, userId, msgId);
            }

            case UpdateType.ChatMember:
            {
                var cm = update.ChatMember;
                if (cm is null) return (null, null, null);

                long? chatId = cm.Chat?.Id;
                long? userId = cm.From?.Id;
                int? msgId = null;

                return (chatId, userId, msgId);
            }

            case UpdateType.MyChatMember:
            {
                var mcm = update.MyChatMember;
                if (mcm is null) return (null, null, null);

                long? chatId = mcm.Chat?.Id;
                long? userId = mcm.From?.Id;
                int? msgId = null;

                return (chatId, userId, msgId);
            }

            default:
                return (null, null, null);
        }
    }
}