using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.FileInputCommandStep;

namespace TeleFlow.Core.Commands.Stateful.StepRender.FileInputCommandStep;

public class FileInputRenderService : IStepRenderService<FileInputStepViewModel>
{
    private readonly FileInputRenderServiceOptions _options;

    public InlineMarkupMessage Render(IServiceProvider serviceProvider, FileInputStepViewModel model)
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