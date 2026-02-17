using Microsoft.Extensions.Logging;
using TeleFlow.Abstractions.Engine.Pipeline;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Polling.Services;

internal class UpdateHandler : IUpdateHandler
{
    private readonly IHandler<Update> _pipeline;
    private readonly ILogger _logger;

    private readonly Action<ILogger, Update>? _beforeUpdate;
    private readonly Action<ILogger, Update>? _afterUpdate;
    private readonly Action<ILogger, Update, Exception> _onUpdateException;
    private readonly Action<ILogger, HandleErrorSource, Exception> _onHandlerException;


    public async Task HandleUpdateAsync(ITelegramBotClient botClient,
                                        Update update,
                                        CancellationToken cancellationToken)
    {
        try
        {
            _beforeUpdate?.Invoke(_logger, update);
            await _pipeline.Handle(update);
            _afterUpdate?.Invoke(_logger, update);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _onUpdateException.Invoke(_logger, update, ex);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient,
                                 Exception exception,
                                 HandleErrorSource source,
                                 CancellationToken cancellationToken)
    {
       _onHandlerException.Invoke(_logger, source, exception);
        return Task.CompletedTask;
    }

    public UpdateHandler(IHandler<Update> pipeline,
                         ILogger logger,
                         Action<ILogger, Update>? beforeUpdate,
                         Action<ILogger, Update>? afterUpdate,
                         Action<ILogger, Update, Exception> onUpdateException,
                         Action<ILogger, HandleErrorSource, Exception> onHandlerException)
    {
        _pipeline = pipeline;
        _logger = logger;
        _beforeUpdate = beforeUpdate;
        _afterUpdate = afterUpdate;
        _onUpdateException = onUpdateException;
        _onHandlerException = onHandlerException;
    }
}