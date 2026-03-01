using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;

public interface IStepRenderService<TViewModel>
    where TViewModel : StepViewModel
{
    InlineMarkupMessage Render(IServiceProvider serviceProvider, TViewModel model);
}