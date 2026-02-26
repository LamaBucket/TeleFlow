using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInput;

public class FileInputCommandStep : ICommandStep
{
    private readonly FileInputCommandStepOptions _options;

    public virtual CommandStepResult GetSuccessStepResult()
        => CommandStepResult.Next;

    public async Task<CommandStepResult> Handle(UpdateContext args)
    {
        var update = args.Update;

        if (update.Message is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoMessageInputMessage);

        var message = update.Message;

        var extractors = args.ServiceProvider.GetServices<IFileReferenceExtractor>().ToList();

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
                    if(fileRef.DeclaredSizeBytes.Value > FileInputCommandStepDefaults.MaxFileSizeBytes)
                        return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.FileExceedsMaxFileSizeMessage);
                }
                else
                {
                    var probeResult = await ProbeFileSizeOk(args.ServiceProvider, fileRef);

                    if(probeResult is not null)
                        return probeResult;
                }
            }

            var ctx = new CommandStepCommitContext(args.ServiceProvider);
            await _options.OnUserCommit(ctx, fileRef);

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

    public async Task OnEnter(IServiceProvider serviceProvider)
    {
        var sender = serviceProvider.GetRequiredService<IMessageSender>();
        await sender.SendMessage(_options.UserPrompt);
    }


    public FileInputCommandStep(FileInputCommandStepOptions options)
    {
        _options = options;
    }
}