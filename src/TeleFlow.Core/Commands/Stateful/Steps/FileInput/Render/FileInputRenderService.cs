using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;

public class FileInputRenderService : IStepRenderService<FileInputStepData>
{
    private readonly FileInputRenderServiceOptions _options;

    public InlineMarkupMessage Render(IServiceProvider serviceProvider, FileInputStepData model)
    {
        string message = "";

        if(model.FileSent is null)
            message = _options.PromptText(serviceProvider);
        else
            if (_options.AfterInputText is not null)
                message = _options.AfterInputText(serviceProvider, model.FileSent);
            else
                message = _options.PromptText(serviceProvider);
        
        return InlineMarkupMessage.CreateTextMessage(message, _options.ParseMode);
    }

    public FileInputRenderService(FileInputRenderServiceOptions options)
    {
        _options = options;
    }
}