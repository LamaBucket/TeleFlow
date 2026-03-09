using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.FileInput;

public class FileInputStep : SingleInputStep<FileReference>
{
    public const long TelegramMaxFileSizeBytes = 20L * 1024 * 1024;

    private readonly FileInputStepOptions _options;


    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<SingleInputStepData<FileReference>> state)
    {
        var update = context.Update;

        if (update.Message is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);

        var message = update.Message;

        var extractors = context.ServiceProvider.GetServices<IFileReferenceExtractor>().ToList();

        if(extractors.Count == 0)
            throw new InvalidOperationException($"No {nameof(IFileReferenceExtractor)} services were registered. Register at least one extractor to enable file input handling.");

        foreach (var extractor in extractors)
        {
            if (!extractor.TryExtract(message, out var fileRef))
                continue;

            if (_options.EnforceMaxFileSize)
            {
                if (fileRef.DeclaredSizeBytes.HasValue)
                {
                    if(fileRef.DeclaredSizeBytes.Value > TelegramMaxFileSizeBytes)
                        return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.FileExceedsMaxFileSizeMessage);
                }
                else
                {
                    var probeResult = await ProbeFileSizeOk(context.ServiceProvider, fileRef);

                    if(probeResult is not null)
                        return probeResult;
                }
            }


            await SetInputAndRerender(context.ServiceProvider, state, fileRef);
            await FinalizeStep(context.ServiceProvider);

            await _options.OnUserCommit(new(context.ServiceProvider), fileRef);

            return GetSuccessStepResult();
        }

        return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoFileProvidedMessage);
    }
   
    private async Task<CommandStepResult?> ProbeFileSizeOk(IServiceProvider sp, FileReference fileRef)
    {
        try
        {
            var botClient = sp.GetRequiredService<ITelegramBotClient>();
            var tgFile = await botClient.GetFileAsync(fileRef.FileId);

            if (string.IsNullOrWhiteSpace(tgFile.FilePath))
                return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.FileNotAvailableMessage);
            
            return null;
        }
        catch (ApiRequestException ex) when (IsFileTooBig(ex))
        {
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.FileExceedsMaxFileSizeMessage);
        }
        catch (ApiRequestException)
        {
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.FileNotAvailableMessage);
        }
    }

    private static bool IsFileTooBig(ApiRequestException ex)
    {
        if (ex.ErrorCode != 400)
            return false;

        return ex.Message.Contains("file is too big", StringComparison.OrdinalIgnoreCase);
    }


    protected virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;


    protected override Task<SingleInputStepData<FileReference>> CreateDefaultStepData(IServiceProvider sp)
        => Task.FromResult(SingleInputStepData<FileReference>.CreateDefault());

    public FileInputStep(FileInputStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}