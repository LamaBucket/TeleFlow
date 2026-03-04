using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput.Render;

internal class ContactInputRenderService : IStepRenderService<ContactInputStepViewModel>
{
    private readonly ContactInputRenderServiceOptions _options;

    public InlineMarkupMessage Render(IServiceProvider serviceProvider, ContactInputStepViewModel model)
    {
        string message = "";

        if(model.ContactShared is null)
            message = _options.PromptText(serviceProvider);
        else
            if (_options.AfterInputText is not null)
                message = _options.AfterInputText(serviceProvider, model.ContactShared);
            else
                message = _options.PromptText(serviceProvider);
        
        return InlineMarkupMessage.CreateTextMessage(message, _options.ParseMode);
    }

    public ContactInputRenderService(ContactInputRenderServiceOptions options)
    {
        _options = options;
    }
}