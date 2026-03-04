using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateful.Steps.TextInput.Render;

internal class TextInputRenderService : IStepRenderService<TextInputStepData>
{
    private readonly TextInputRenderServiceOptions _options;

    public InlineMarkupMessage Render(IServiceProvider serviceProvider, TextInputStepData model)
    {
        string message = "";

        if(model.TextEntered is null)
            message = _options.PromptText(serviceProvider);
        else
            if (_options.AfterInputText is not null)
                message = _options.AfterInputText(serviceProvider, model.TextEntered);
            else
                message = _options.PromptText(serviceProvider);
                
        return InlineMarkupMessage.CreateTextMessage(message, _options.ParseMode);
    }

    public TextInputRenderService(TextInputRenderServiceOptions options)
    {
        _options = options;
    }
}