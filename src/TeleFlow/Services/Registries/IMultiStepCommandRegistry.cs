using TeleFlow.Factories;

namespace TeleFlow.Services.Registries;

public interface IMultiStepCommandRegistry
{
    StepCommandFactory GetFactoryFor(string commandName);
}