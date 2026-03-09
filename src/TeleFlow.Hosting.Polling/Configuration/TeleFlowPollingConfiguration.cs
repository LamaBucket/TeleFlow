using Microsoft.Extensions.Logging;
using TeleFlow.Hosting.Polling.Configuration.Internal;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Hosting.Polling.Configuration;

public class TeleFlowPollingConfiguration
{
    public required string BotToken { get; init; }

    public string ClientName { get; init; } = "teleflow-telegram";

    public UpdateType[] AllowedUpdates { get; init; } = [];

    public Action<ILogger, Update>? BeforeUpdate { get; init; }
    public Action<ILogger, Update>? AfterUpdate { get; init; }
    
    public Action<ILogger, Update, Exception> OnUpdateException { get; init; } = TeleFlowPollingDefaultExceptionHandlers.OnUpdateExceptionDefault;

    public Action<ILogger, HandleErrorSource, Exception> OnHandlerException { get; init; } = TeleFlowPollingDefaultExceptionHandlers.OnHandlerExceptionDefault;
}