using TeleFlow.Commands.MultiStep;
using TeleFlow.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace TeleFlow.Services;

public interface IStateValidationDisplayNameProvider
{
    IHandlerFactoryWithContext<StepCommand, Update, StepCommand> StepToRestartCommandFactory { get; }
    
    string GetDisplayName();
}