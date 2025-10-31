
using TeleFlow.Services;
using TeleFlow.Services.InputValidators;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Commands.MultiStep.StepCommands;

public class ImageInputStepCommand : StepCommandWithValidation
{
    private readonly IMessageService<string> _messageService;

    private readonly string _onCommandCreatedMessage;


    private readonly IMediaDownloaderService _mediaDownloaderService;

    private readonly Func<byte[], Task> _onHandleUserImage;    


    public override async Task OnCommandCreated()
    {
        await _messageService.SendMessage(_onCommandCreatedMessage);
    }

    protected override async Task HandleValidInput(Update args)
    {
        if (args.Type != UpdateType.Message || args.Message is null || args.Message.Photo is null)
            throw new NullReferenceException();

        var photo = args.Message.Photo.Last();

        var file = await _mediaDownloaderService.DownloadFileAsync(photo.FileId);

        await _onHandleUserImage.Invoke(file);

        await FinalizeCommand();
    }


    public ImageInputStepCommand(StepCommand? next,
                                 IMessageService<string> messageService,
                                 string onCommandCreatedMessage,
                                 IMediaDownloaderService mediaDownloaderService,
                                 Func<byte[], Task> onHandleUserImage,
                                 IUserInputValidator? userInputValidator = null) : base(next, userInputValidator)
    {
        _messageService = messageService;
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _mediaDownloaderService = mediaDownloaderService;
        _onHandleUserImage = onHandleUserImage;
    }
}