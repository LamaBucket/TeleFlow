using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;

namespace TeleFlow.Core.Commands.Stateful.Steps.ViewModelStepBase;

public class ViewModelCommandStepBaseOptions<TViewModel>
    where TViewModel : class
{
    public required IStepRenderService<TViewModel> RenderService { get; init; }

    public string StateExpiredMessage { get; init; } = "Your State Expired.";
    
}