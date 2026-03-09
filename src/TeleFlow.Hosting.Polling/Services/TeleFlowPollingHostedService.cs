using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeleFlow.Abstractions.Engine.Pipeline;
using TeleFlow.Hosting.Polling.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TeleFlow.Hosting.Polling.Services;

internal sealed class TeleFlowPollingHostedService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IHandler<Update> _pipeline;
    private readonly TeleFlowPollingConfiguration _cfg;
    private readonly ILogger<TeleFlowPollingHostedService> _logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = _cfg.AllowedUpdates
        };

        var handler = new UpdateHandler(_pipeline,
                                        _logger,
                                        _cfg.BeforeUpdate,
                                        _cfg.AfterUpdate,
                                        _cfg.OnUpdateException,
                                        _cfg.OnHandlerException);
        
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("TeleFlow polling receiver started");
        }

        _botClient.StartReceiving(updateHandler: handler,
                                  receiverOptions: receiverOptions,
                                  cancellationToken: stoppingToken);

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("TeleFlow polling receiver stopped.");
            tcs.TrySetResult();
        });

        return tcs.Task;
    }

        public TeleFlowPollingHostedService(ITelegramBotClient botClient,
                                            IHandler<Update> pipeline,
                                            TeleFlowPollingConfiguration cfg,
                                            ILogger<TeleFlowPollingHostedService> logger)
        {
            _botClient = botClient;
            _pipeline = pipeline;
            _cfg = cfg;
            _logger = logger;
        }
}
