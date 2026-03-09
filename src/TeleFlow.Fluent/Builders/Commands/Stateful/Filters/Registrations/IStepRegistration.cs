using TeleFlow.Abstractions.Engine.Commands.Stateful;

namespace TeleFlow.Fluent.Builders.Commands.Stateful.Filters.Registrations;

internal interface IStepRegistration
{
    CommandStepFactory CompileStepFactory();
}