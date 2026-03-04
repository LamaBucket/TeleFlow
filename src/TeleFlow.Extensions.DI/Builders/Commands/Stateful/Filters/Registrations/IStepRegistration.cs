using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters.Registrations;

internal interface IStepRegistration
{
    CommandStepFactory CompileStepFactory();
}