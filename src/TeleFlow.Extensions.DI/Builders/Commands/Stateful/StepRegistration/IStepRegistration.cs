using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.StepRegistration;

internal interface IStepRegistration
{
    Func<ICommandStep> CompileFactory();
}