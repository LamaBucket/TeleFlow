using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInput;

public class FileInputCommandStep : StatefulStep<FileInputStepData>
{
    public const long TelegramMaxFileSizeBytes = 20L * 1024 * 1024;

    private readonly FileInputCommandStepOptions _options;


    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<FileInputStepData> state)
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


            await SetStateInputTextAndRerender(context.ServiceProvider, state, fileRef);
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

    private async Task SetStateInputTextAndRerender(IServiceProvider sp, StepState<FileInputStepData> state, FileReference value)
    {
        state = state with
        {
            ViewModel = state.ViewModel with
            {
                FileSent = value
            }
        };

        await UpsertAndRerender(sp, state);
    }

    public virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;


    protected override Task<FileInputStepData> CreateDefaultViewModel(IServiceProvider sp)
        => Task.FromResult(FileInputStepData.Default);

    public FileInputCommandStep(FileInputCommandStepOptions options) : base(options.RenderConfig)
    {
        _options = options;
    }
}