using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;

public interface IStepRenderService<in TModel>
{
    InlineMarkupMessage Render(IServiceProvider serviceProvider, TModel model);
}