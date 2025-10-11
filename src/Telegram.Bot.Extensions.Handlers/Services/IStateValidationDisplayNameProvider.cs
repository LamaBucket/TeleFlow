using Telegram.Bot.Extensions.Handlers.Commands.MultiStep;
using Telegram.Bot.Extensions.Handlers.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services;

public interface IStateValidationDisplayNameProvider
{
    IHandlerFactoryWithArgs<StepCommand, Update, StepCommand> StepToRestartCommandFactory { get; }
    
    string GetDisplayName();
}