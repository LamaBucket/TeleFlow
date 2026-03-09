using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base.Render;

public class SingleInputRenderService<TInput>(SingleInputRenderOptions<TInput> options)
    : IStepRenderService<SingleInputStepData<TInput>>
{
    public InlineMarkupMessage Render(IServiceProvider serviceProvider, SingleInputStepData<TInput> data)
    {
        var message = options.UserPrompt(serviceProvider, data.Input);
                
        return InlineMarkupMessage.CreateTextMessage(message, options.ParseMode);
    }
}