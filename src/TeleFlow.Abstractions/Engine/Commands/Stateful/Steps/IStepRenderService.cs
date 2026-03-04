using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;

public interface IStepRenderService<TData>
    where TData : StepData
{
    InlineMarkupMessage Render(IServiceProvider serviceProvider, TData data);
}