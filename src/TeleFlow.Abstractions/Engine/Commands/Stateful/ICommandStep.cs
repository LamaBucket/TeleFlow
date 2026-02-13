using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;

namespace TeleFlow.Abstractions.Engine.Commands.Stateful;

public interface ICommandStep
{
    Task<CommandStepResult> Handle(UpdateContext args);

    Task OnEnter(IServiceProvider serviceProvider);
}